using Einstein.config;
using Einstein.config.bibiteVersions;
using Einstein.model;
using Einstein.model.json;
using Einstein.ui;
using Einstein.ui.editarea;
using Einstein.ui.menu;
using phi.control;
using phi.graphics.drawables;
using phi.io;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using static phi.graphics.drawables.Text;

namespace Einstein
{
    public class EditorScene : Scene
    {
        // ----------------------------------------------------------------
        //  Config
        // ----------------------------------------------------------------

        private static readonly string DEFAULT_SAVE_LOAD_FOLDER =
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
            + "\\AppData\\LocalLow\\The Bibites\\The Bibites\\Bibites";

        // ----------------------------------------------------------------
        //  Data/Constructor
        // ----------------------------------------------------------------

        private BibiteVersion bibiteVersion;

        private EditArea editArea;

        // TODO maybe refactor the input/output/hidden buttons into one renderable class
        private NeuronMenuCategory selected;
        private IONeuronMenuCategory input;
        private IONeuronMenuCategory output;
        private HiddenNeuronMenuCategory hidden;
        private Button loadButton;
        private Button saveButton;
        private SaveMessageText saveMessageText;
        private Button autoArrangeButton;
        private KeybindsInfoText infoText;
        private Text bibiteVersionText;
        private ZoomControls zoomControls;

        private string savePath;
        private string loadPath;
        private int prevWindowWidth;
        private int prevWindowHeight;

        public EditorScene(Scene prevScene) : base(prevScene, EinsteinConfig.COLOR_MODE.Background)
        {
            bibiteVersion = BibiteVersion.DEFAULT_VERSION;

            editArea = new EditArea(new JsonBrain(bibiteVersion), moveNeuronIntoMenu, bibiteVersion);

            selected = null;
            NeuronMenuButton inputButton = new NeuronMenuButton(
                EinsteinConfig.PAD,
                EinsteinConfig.PAD,
                "Input Neurons",
                onSelectInputs,
                onDeselectInputs);
            input = new IONeuronMenuCategory(
                inputButton,
                bibiteVersion.InputNeurons,
                moveNeuronIntoEditArea);

            NeuronMenuButton outputButton = new NeuronMenuButton(
                EinsteinConfig.PAD,
                2 * EinsteinConfig.PAD + NeuronMenuButton.HEIGHT,
                "Output Neurons",
                onSelectOutputs,
                onDeselectOutputs);
            output = new IONeuronMenuCategory(
                outputButton,
                bibiteVersion.OutputNeurons,
                moveNeuronIntoEditArea);

            NeuronMenuButton hiddenButton = new NeuronMenuButton(
                EinsteinConfig.PAD,
                3 * EinsteinConfig.PAD + 2 * NeuronMenuButton.HEIGHT,
                "Hidden Neurons",
                onSelectAdd,
                onDeselectAdd);
            hidden = new HiddenNeuronMenuCategory(
                hiddenButton,
                bibiteVersion.HiddenNeurons,
                createHiddenNeuronInEditArea);
            loadButton = new Button.ButtonBuilder(
                new ImageWrapper(NeuronMenuButton.UNSELECTED_IMAGE_PATH),
                EinsteinConfig.PAD,
                5 * EinsteinConfig.PAD + 3 * NeuronMenuButton.HEIGHT)
                .withText("Load from Bibite")
                .withOnClick(loadBrain)
                .Build();
            saveButton = new Button.ButtonBuilder(
                new ImageWrapper(NeuronMenuButton.UNSELECTED_IMAGE_PATH),
                EinsteinConfig.PAD,
                6 * EinsteinConfig.PAD + 4 * NeuronMenuButton.HEIGHT)
                .withText("Save to Bibite")
                .withOnClick(saveBrain)
                .Build();
            bibiteVersionText = new TextBuilder(getBb8VersionText())
                .WithColor(new SolidBrush(EinsteinConfig.COLOR_MODE.Text))
                .WithFontSize(10f)
                .WithXY(EinsteinConfig.PAD,
                    EinsteinConfig.PAD + saveButton.GetY() + saveButton.GetHeight())
                .Build();
            saveMessageText = new SaveMessageText(
                EinsteinConfig.PAD + NeuronMenuButton.WIDTH / 2,
                EinsteinConfig.PAD + bibiteVersionText.GetY() + bibiteVersionText.GetHeight());
            autoArrangeButton = new Button.ButtonBuilder(
                new ImageWrapper(NeuronMenuButton.UNSELECTED_IMAGE_PATH),
                EinsteinConfig.PAD,
                EinsteinConfig.PAD + saveMessageText.GetY() + saveMessageText.GetHeight())
                .withText("Auto-Arrange")
                .withOnClick(editArea.AutoArrange)
                .Build();

            infoText = new KeybindsInfoText(
                EinsteinConfig.PAD,
                20 + EinsteinConfig.PAD + autoArrangeButton.GetY() + autoArrangeButton.GetHeight());
            zoomControls = new ZoomControls(editArea);

            savePath = null;
            loadPath = null;
            prevWindowWidth = EinsteinConfig.Window.INITIAL_WIDTH;
            prevWindowHeight = EinsteinConfig.Window.INITIAL_HEIGHT;
        }

        // ----------------------------------------------------------------
        //  Initialize and Close
        // ----------------------------------------------------------------

        protected override void InitializeMe()
        {
            editArea.Initialize();
            input.Initialize();
            output.Initialize();
            hidden.Initialize();
            loadButton.Initialize();
            saveButton.Initialize();
            autoArrangeButton.Initialize();
            zoomControls.Initialize();
            IO.RENDERER.Add(loadButton);
            IO.RENDERER.Add(saveButton);
            IO.RENDERER.Add(autoArrangeButton);
            IO.RENDERER.Add(bibiteVersionText);
            IO.RENDERER.Add(infoText);
            IO.FRAME_TIMER.Subscribe(checkForResize);
        }

        protected override void UninitializeMe()
        {
            editArea.Uninitialize();
            input.Uninitialize();
            output.Uninitialize();
            hidden.Uninitialize();
            loadButton.Uninitialize();
            saveButton.Uninitialize();
            autoArrangeButton.Uninitialize();
            zoomControls.Uninitialize();
            IO.RENDERER.Remove(loadButton);
            IO.RENDERER.Remove(saveButton);
            IO.RENDERER.Remove(autoArrangeButton);
            IO.RENDERER.Remove(bibiteVersionText);
            IO.RENDERER.Remove(infoText);
            IO.FRAME_TIMER.Unsubscribe(checkForResize);
        }

        // ----------------------------------------------------------------
        //  Behavior
        // ----------------------------------------------------------------

        // TODO look at increasing performance of re-clicking the same drawable very quickly?

        // ----- Saving/Loading -----

        private void saveBrain()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }

            // TODO check bb8Version of the destination .bb8 file and do conversion handling

            // set inov positioning
            foreach (NeuronRenderable nr in editArea.NeuronRenderables)
            {
                int x = nr.NeuronDrawable.GetCenterX();
                int y = nr.NeuronDrawable.GetCenterY();
                ((JsonNeuron)nr.Neuron).SetInovXY(x, y);
            }

            // read file
            string filepath = IO.POPUPS.PromptForFile(getSavePath(), "Bibite Files|*.bb8",
                "Save to Bibite", "");
            if (filepath == "")
            {
                // User cancelled
                return;
            }
            if (!File.Exists(filepath))
            {
                IO.POPUPS.ShowErrorPopup("Save Failed", "File not found.");
                return;
            }
            string json = File.ReadAllText(filepath);

            // determine where to insert the brain
            int startIndex = json.IndexOf("\"brain\":") + "\"brain\":".Length;
            int endIndex = json.IndexOf("\"immuneSystem\":");
            if (startIndex < 0 || endIndex < 0)
            {
                IO.POPUPS.ShowErrorPopup("Save Failed", "File format is invalid: Cannot find an existing brain.");
                return;
            }

            // determine destination version
            string versionName;
            try
            {
                versionName = parseVersionName(json);
            }
            catch (NoNextValueException e)
            {
                IO.POPUPS.ShowErrorPopup("Save Failed", "File format is invalid: Cannot find Bibites version.\nDetails: " + e.Message);
                return;
            }
            BibiteVersion targetBibiteVersion;
            try
            {
                targetBibiteVersion = BibiteVersion.FromName(versionName);
            }
            catch (NoSuchVersionException e)
            {
                showVersionMismatchErrorPopup("Save Failed", e);
                return;
            }

            BaseBrain brainToSave = editArea.Brain;

            // ask the user if they want to convert
            if (targetBibiteVersion != bibiteVersion)
            {
                bool answer = IO.POPUPS.ShowYesNoPopup("Version Mismatch",
                    $"This brain's Bibites version is '{bibiteVersion}'.\n" +
                    $"The chosen .bb8 file's Bibites version is '{targetBibiteVersion}'.\n\n" +
                    $"Do you want Einstein to try automatically converting the brain to version '{targetBibiteVersion}' and save it to the file?\n\n" +
                    "Regardless of your choice, the brain will remain open in the editor in its unconverted state.");
                if (!answer)
                {
                    return;
                }
                try
                {
                    brainToSave = bibiteVersion.CreateConvertedCopyOf(brainToSave, targetBibiteVersion);
                }
                catch (CannotConvertException e)
                {
                    IO.POPUPS.ShowErrorPopup("Unable to convert brain to "
                        + targetBibiteVersion.VERSION_NAME, e.Message);
                    return;
                }
            }

            string brainJson = ((JsonBrain)brainToSave).GetSave(targetBibiteVersion);
            json = json.Substring(0, startIndex) + " " + brainJson + json.Substring(endIndex);
            File.WriteAllText(filepath, json);
            savePath = Path.GetDirectoryName(filepath);
            saveMessageText.Show();
        }

        private void loadBrain()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }

            // read the file
            string filepath = IO.POPUPS.PromptForFile(getLoadPath(), "Bibite Files|*.bb8",
                "Load from Bibite", "");
            if (filepath == "") { return; }
            if (!File.Exists(filepath))
            {
                IO.POPUPS.ShowErrorPopup("Load Failed", "File not found.");
                return;
            }
            string json = File.ReadAllText(filepath);

            // parse and set the version number
            string newBibiteVersionName;
            try
            {
                newBibiteVersionName = parseVersionName(json);
            }
            catch (JsonParsingException e)
            {
                IO.POPUPS.ShowErrorPopup("Load Failed",
                    "File format is invalid. Cannot find Bibites version.\n\nDetails: " + e.Message);
                return;
            }
            BibiteVersion newBibiteVersion;
            try
            {
                newBibiteVersion = BibiteVersion.FromName(newBibiteVersionName);
            }
            catch (NoSuchVersionException e)
            {
                showVersionMismatchErrorPopup("Load Failed", e);
                return;
            }

            // parse and set the brain
            JsonBrain brain;
            bool autoFixNeuronDescriptions = false;
            while (true)
            {
                try
                {
                    brain = new JsonBrain(json, json.IndexOf("\"brain\":") + 8, newBibiteVersion);
                    break;
                }
                catch (JsonParsingException e)
                {
                    IO.POPUPS.ShowErrorPopup("Load Failed",
                        "File format is invalid.\n\nDetails: " + e.Message);
                    return;
                }
                catch (ContainsDuplicateNeuronDescriptionException e)
                {
                    autoFixNeuronDescriptions = autoFixNeuronDescriptions ||
                        IO.POPUPS.ShowYesNoPopup("Load Failed", e.GetDisplayMessage(": ") +
                        "\n\nDo you want Einstein to fix this by assigning a new description " +
                        "to this neuron and to any other duplicate descriptions that are found? " +
                        "\n(This could take a long time for big brains.)");
                    if (!autoFixNeuronDescriptions) { return; }

                    string originalDescription = e.Neuron.Description;
                    string nonNumberDesc = originalDescription.TrimEnd('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
                    string descNumberStr = originalDescription.Substring(nonNumberDesc.Length);
                    // if last 8 characters of nonNumberDesc are not 'AutoDesc'
                    if (nonNumberDesc.Length <= 8 || nonNumberDesc.Substring(nonNumberDesc.Length - 8, 8) != "AutoDesc")
                    {
                        nonNumberDesc += "AutoDesc";
                    }

                    int descNumberInt = int.TryParse(descNumberStr, out int parseResult) ? parseResult : 0;
                    string newDescription = nonNumberDesc + descNumberInt;
                    while (json.Contains(newDescription))
                    {
                        descNumberInt++;
                        newDescription = nonNumberDesc + descNumberInt;
                    }
                    // replace 2nd instance of the original description with the new description
                    int indexOf2ndInstance = 1 + json.IndexOf("\"" + originalDescription + "\"",
                        json.IndexOf("\"" + originalDescription + "\""));
                    json = json.Substring(0, indexOf2ndInstance) +
                        newDescription +
                        json.Substring(indexOf2ndInstance + originalDescription.Length);
                    continue;
                }
                catch (BrainException e)
                {
                    IO.POPUPS.ShowErrorPopup("Load Failed",
                        "File format is correct but some values are invalid.\n\n" +
                        e.GetDisplayMessage(": "));
                    return;
                }
            }

            // only on successful load...
            bibiteVersion = newBibiteVersion;
            bibiteVersionText.SetMessage(getBb8VersionText());
            editArea.LoadBrain(brain, newBibiteVersion);
            updateIONeuronsInMenu(); // depends on the brain being set in editArea.LoadBrain
            hidden.ResetNeuronOptionsTo(bibiteVersion.HiddenNeurons);
            loadPath = Path.GetDirectoryName(filepath);
        }

        private string parseVersionName(string json)
        {
            const string versionTag = "\"version\":";
            int indexOfVersionTag = json.IndexOf(versionTag);
            if (indexOfVersionTag < 0)
            {
                throw new NoNextValueException("Version not found");
            }
            int indexLeft = json.IndexOf('"', indexOfVersionTag + versionTag.Length) + 1;
            if (indexLeft < 0)
            {
                throw new NoNextValueException("No '\"' found after index " + indexOfVersionTag + versionTag.Length);
            }
            int indexRight = json.IndexOf('"', indexLeft);
            if (indexRight < 0)
            {
                throw new NoNextValueException("No '\"' found after index " + indexLeft);
            }
            int length = indexRight - indexLeft;
            return json.Substring(indexLeft, length);
        }

        private static void showVersionMismatchErrorPopup(string title, NoSuchVersionException e)
        {
            IO.POPUPS.ShowErrorPopup(title, e.Message + "\n\n" +
                    "This bibite's version is either invalid or is too old or too new to be recognized by Einstein.\n" +
                    "If it's too new, consider checking for updates to Einstein at https://github.com/quaris628/EinsteinEditor/releases/\n\n" +
                    "If, for some reason, you really really really want to try editing this bibite anyway, " +
                    "beware! You will probably run into seriously bad problems like data corruption, " +
                    "unpredictable crashes, and bugged 'god' or 'demon' bibites... but technically you can change the " +
                    "bibite file's version number to trick Einstein into treating the file as if it was that version. " +
                    "As long as you back up your .bb8 files beforehand, there won't be any permanent harm done. " +
                    "Again, I don't advise doing this, but it is a last-resort option.");
        }

        private string getSavePath()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            if (savePath != null) { return savePath; }
            if (loadPath != null) { return loadPath; }
            return DEFAULT_SAVE_LOAD_FOLDER;
        }

        private string getLoadPath()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            if (loadPath != null) { return loadPath; }
            if (savePath != null) { return savePath; }
            return DEFAULT_SAVE_LOAD_FOLDER;
        }

        private void updateIONeuronsInMenu()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            input.ClearAllOptions();
            output.ClearAllOptions();
            foreach (BaseNeuron neuron in bibiteVersion.InputNeurons)
            {
                if (!editArea.Brain.ContainsNeuron(neuron))
                {
                    input.AddOption(neuron);
                }
            }
            foreach (BaseNeuron neuron in bibiteVersion.OutputNeurons)
            {
                if (!editArea.Brain.ContainsNeuron(neuron))
                {
                    output.AddOption(neuron);
                }
            }
        }

        // ----- Moving between edit area and menu -----

        // upon neuron being removed from edit area
        private void moveNeuronIntoMenu(BaseNeuron neuronRemovedFromEditArea)
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            if (neuronRemovedFromEditArea.IsInput())
            {
                input.AddOption(neuronRemovedFromEditArea);
            }
            else if (neuronRemovedFromEditArea.IsOutput())
            {
                output.AddOption(neuronRemovedFromEditArea);
            }
        }

        private void moveNeuronIntoEditArea(BaseNeuron neuronRemovedFromMenu)
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            editArea.AddNeuron(neuronRemovedFromMenu);
        }
        private void createHiddenNeuronInEditArea(BaseNeuron hiddenNeuronToAdd)
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            editArea.CreateHiddenNeuron(hiddenNeuronToAdd.Type);
        }

        // ----- Button Selection -----

        private void onSelectInputs()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            selected?.Button.Deselect();
            selected = input;
        }
        private void onSelectOutputs()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            selected?.Button.Deselect();
            selected = output;
        }
        private void onSelectAdd()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            selected?.Button.Deselect();
            selected = hidden;
        }

        private void onDeselectInputs()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            selected = null;
        }
        private void onDeselectOutputs()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            selected = null;
        }
        private void onDeselectAdd()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            selected = null;
        }

        // ----- other/misc -----

        private string getBb8VersionText()
        {
            return "Bibite Version: " + bibiteVersion.VERSION_NAME;
        }

        // check if the window has been resized
        // if so, we probably need to reposition the menu buttons
        private void checkForResize()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            int newWidth = IO.WINDOW.GetWidth();
            int newHeight = IO.WINDOW.GetHeight();

            if (newHeight <= 0)
            {
                return;
            }

            if (newWidth != prevWindowWidth)
            {
                input.RepositionOptions();
                output.RepositionOptions();
                hidden.RepositionOptions();
                prevWindowWidth = newWidth;
            }
            if (newHeight != prevWindowHeight)
            {
                zoomControls.Reposition();
                prevWindowHeight = newHeight;
            }
        }

        public string LogDetailsForCrash()
        {
            string log = "Version = " + EinsteinConfig.VERSION;
            log += "\nbibiteVersion = " + (bibiteVersion == null ? "null" : bibiteVersion.ToString());
            log += "\nEditorScene.editArea = " + editArea.LogDetailsForCrash();
            log += "\nselected = ";
            if (selected == null)
            {
                log += "null";
            }
            else if (selected == input)
            {
                log += "input";
            }
            else if (selected == output)
            {
                log += "output";
            }
            else if (selected == hidden)
            {
                log += "hidden";
            }
            log += "\ninput = " + input.LogDetailsForCrash();
            log += "\noutput = " + output.LogDetailsForCrash();
            log += "\nhidden = " + hidden.LogDetailsForCrash();
            log += "\nsavePath = " + (savePath ?? "null");
            log += "\nloadPath = " + (loadPath ?? "null");
            log += "\nprevWindowWidth = " + prevWindowWidth;
            log += "\nisInit = " + isInit;
            return log;
        }
    }
}
