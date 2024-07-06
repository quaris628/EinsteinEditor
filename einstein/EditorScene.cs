using Einstein.config;
using Einstein.config.bibiteVersions;
using Einstein.model;
using Einstein.model.json;
using Einstein.ui;
using Einstein.ui.editarea;
using Einstein.ui.menu;
using Einstein.ui.menu.categories.colors;
using phi.control;
using phi.graphics.drawables;
using phi.io;
using phi.other;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private const string HELP_FILE_PATH = EinsteinConfig.HOME_DIR + "help.txt";

        // ----------------------------------------------------------------
        //  Data/Constructor
        // ----------------------------------------------------------------

        private BibiteVersion bibiteVersion;

        private EditArea editArea;

        // TODO maybe refactor the input/output/hidden buttons into one renderable class
        private MenuCategory selected;
        private IONeuronMenuCategory input;
        private HiddenNeuronMenuCategory hidden;
        private IONeuronMenuCategory output;
        private ColorMenuCategory editColorMenuCategory;
        private RectangleDrawable paintColorDisplay;
        private Button loadButton;
        private Button resaveButtonDisabledPlaceholder;
        private Button resaveButton;
        private Text bibiteNameText;
        private Text bibiteVersionText;
        private Button saveToButton;
        private SaveMessageText saveMessageText;
        private Button autoArrangeButton;
        private Button helpButton;
        private ZoomControls zoomControls;

        private string savePath;
        private string loadPath;
        private string mostRecentSavedToFile;
        private string mostRecentLoadedToFile;
        private int prevWindowWidth;
        private int prevWindowHeight;

        public EditorScene(Scene prevScene) : base(prevScene, EinsteinConfig.COLOR_MODE.Background)
        {
            bibiteVersion = BibiteVersion.DEFAULT_VERSION;

            editArea = new EditArea(new JsonBrain(bibiteVersion), moveNeuronIntoMenu, bibiteVersion);

            selected = null;
            MenuCategoryButton inputButton = new MenuCategoryButton(
                EinsteinConfig.PAD,
                EinsteinConfig.PAD,
                "Inputs",
                onSelectInputs,
                onDeselectInputs);
            input = new IONeuronMenuCategory(
                inputButton,
                bibiteVersion.InputNeurons,
                moveNeuronIntoEditArea);
            MenuCategoryButton outputButton = new MenuCategoryButton(
                EinsteinConfig.PAD,
                EinsteinConfig.PAD + inputButton.GetY() + inputButton.GetHeight(),
                "Outputs",
                onSelectOutputs,
                onDeselectOutputs);
            output = new IONeuronMenuCategory(
                outputButton,
                bibiteVersion.OutputNeurons,
                moveNeuronIntoEditArea);
            MenuCategoryButton hiddenButton = new MenuCategoryButton(
                EinsteinConfig.PAD,
                EinsteinConfig.PAD + outputButton.GetY() + outputButton.GetHeight(),
                "Hidden",
                onSelectHidden,
                onDeselectHidden);
            hidden = new HiddenNeuronMenuCategory(
                hiddenButton,
                bibiteVersion.HiddenNeurons,
                createHiddenNeuronInEditArea);

            editArea.PaintColor = ColorMenuCategory.STARTING_COLOR;
            EditColorMenuCategoryButton editColorButton = new EditColorMenuCategoryButton(
                EinsteinConfig.PAD,
                EinsteinConfig.PAD + hiddenButton.GetY() + hiddenButton.GetHeight(),
                "Paint Neurons",
                onSelectColors,
                onDeselectColors);
            editColorMenuCategory = new ColorMenuCategory(
                editColorButton,
                (color) => {
                    editArea.PaintColor = color;
                    paintColorDisplay.SetPen(new Pen(color));
                });
            paintColorDisplay = new RectangleDrawable(EinsteinConfig.PAD,
                EinsteinConfig.PAD + hiddenButton.GetY() + hiddenButton.GetHeight(),
                EditColorMenuCategoryButton.WIDTH, EditColorMenuCategoryButton.HEIGHT);
            paintColorDisplay.SetPen(new Pen(editArea.PaintColor));

            loadButton = new Button.ButtonBuilder(
                new ImageWrapper(MenuCategoryButton.UNSELECTED_IMAGE_PATH),
                EinsteinConfig.PAD,
                EinsteinConfig.PAD * 2 + paintColorDisplay.GetY() + paintColorDisplay.GetHeight())
                .withText("Open .bb8")
                .withOnClick(loadFromBibite)
                .Build();
            resaveButtonDisabledPlaceholder = new Button.ButtonBuilder(new ImageWrapper(MenuCategoryButton.UNSELECTED_IMAGE_PATH),
                EinsteinConfig.PAD,
                EinsteinConfig.PAD + loadButton.GetY() + loadButton.GetHeight())
                .withText(new TextBuilder("Save").WithColor(new SolidBrush(EinsteinConfig.COLOR_MODE.DisabledButtonText)).Build())
                .withOnClick(() => { })
                .Build();
            resaveButton = new Button.ButtonBuilder(new ImageWrapper(MenuCategoryButton.UNSELECTED_IMAGE_PATH),
                resaveButtonDisabledPlaceholder.GetX(),
                resaveButtonDisabledPlaceholder.GetY())
                .withText("Save")
                .withOnClick(resaveToBibite)
                .Build();
            editArea.Brain.OnChangeFlagged += (_container) => {
                resaveButton.SetDisplaying(getResaveTargetFile() != null);
            };
            resaveButton.SetDisplaying(false);
            saveToButton = new Button.ButtonBuilder(
                new ImageWrapper(MenuCategoryButton.UNSELECTED_IMAGE_PATH),
                EinsteinConfig.PAD,
                EinsteinConfig.PAD + resaveButton.GetY() + resaveButton.GetHeight())
                .withText("Save as")
                .withOnClick(saveToBibite)
                .Build();
            bibiteNameText = new TextBuilder(getBb8NameText())
                .WithColor(new SolidBrush(EinsteinConfig.COLOR_MODE.Text))
                .WithFontSize(10f)
                .WithXY(EinsteinConfig.PAD,
                    EinsteinConfig.PAD + saveToButton.GetY() + saveToButton.GetHeight())
                .Build();
            bibiteVersionText = new TextBuilder(getBb8VersionText())
                .WithColor(new SolidBrush(EinsteinConfig.COLOR_MODE.Text))
                .WithFontSize(10f)
                .WithXY(EinsteinConfig.PAD,
                    EinsteinConfig.PAD + bibiteNameText.GetY() + bibiteNameText.GetHeight())
                .Build();
            saveMessageText = new SaveMessageText(
                EinsteinConfig.PAD + MenuCategoryButton.WIDTH / 2,
                EinsteinConfig.PAD + bibiteVersionText.GetY() + bibiteVersionText.GetHeight());

            autoArrangeButton = new Button.ButtonBuilder(
                new ImageWrapper(MenuCategoryButton.UNSELECTED_IMAGE_PATH),
                EinsteinConfig.PAD,
                EinsteinConfig.PAD * 2 + saveMessageText.GetY() + saveMessageText.GetHeight())
                .withText("Auto-Arrange")
                .withOnClick(editArea.AutoArrange)
                .Build();

            helpButton = new Button.ButtonBuilder(
                    new ImageWrapper(MenuCategoryButton.UNSELECTED_IMAGE_PATH),
                    EinsteinConfig.PAD,
                    EinsteinConfig.PAD + autoArrangeButton.GetY() + autoArrangeButton.GetHeight())
                    .withText("Help")
                    .withOnClick(openHelp)
                    .Build();

            zoomControls = new ZoomControls(editArea);

            savePath = null;
            loadPath = null;
            mostRecentSavedToFile = null;
            mostRecentLoadedToFile = null;
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
            hidden.Initialize();
            output.Initialize();
            editColorMenuCategory.Initialize();
            IO.RENDERER.Add(paintColorDisplay, EinsteinConfig.Render.DEFAULT_LAYER);
            IO.KEYS.Subscribe(RebindColorShortcut1, (int)EinsteinConfig.Keybinds.SELECT_PAINT_COLORS[0] + (int)EinsteinConfig.Keybinds.REBIND_PAINT_COLOR_MODIFIER);
            IO.KEYS.Subscribe(RebindColorShortcut2, (int)EinsteinConfig.Keybinds.SELECT_PAINT_COLORS[1] + (int)EinsteinConfig.Keybinds.REBIND_PAINT_COLOR_MODIFIER);
            IO.KEYS.Subscribe(RebindColorShortcut3, (int)EinsteinConfig.Keybinds.SELECT_PAINT_COLORS[2] + (int)EinsteinConfig.Keybinds.REBIND_PAINT_COLOR_MODIFIER);
            IO.KEYS.Subscribe(RebindColorShortcut4, (int)EinsteinConfig.Keybinds.SELECT_PAINT_COLORS[3] + (int)EinsteinConfig.Keybinds.REBIND_PAINT_COLOR_MODIFIER);
            IO.KEYS.Subscribe(RebindColorShortcut5, (int)EinsteinConfig.Keybinds.SELECT_PAINT_COLORS[4] + (int)EinsteinConfig.Keybinds.REBIND_PAINT_COLOR_MODIFIER);
            IO.KEYS.Subscribe(RebindColorShortcut6, (int)EinsteinConfig.Keybinds.SELECT_PAINT_COLORS[5] + (int)EinsteinConfig.Keybinds.REBIND_PAINT_COLOR_MODIFIER);
            IO.KEYS.Subscribe(RebindColorShortcut7, (int)EinsteinConfig.Keybinds.SELECT_PAINT_COLORS[6] + (int)EinsteinConfig.Keybinds.REBIND_PAINT_COLOR_MODIFIER);
            IO.KEYS.Subscribe(RebindColorShortcut8, (int)EinsteinConfig.Keybinds.SELECT_PAINT_COLORS[7] + (int)EinsteinConfig.Keybinds.REBIND_PAINT_COLOR_MODIFIER);
            IO.KEYS.Subscribe(RebindColorShortcut9, (int)EinsteinConfig.Keybinds.SELECT_PAINT_COLORS[8] + (int)EinsteinConfig.Keybinds.REBIND_PAINT_COLOR_MODIFIER);
            IO.KEYS.Subscribe(RebindColorShortcut0, (int)EinsteinConfig.Keybinds.SELECT_PAINT_COLORS[9] + (int)EinsteinConfig.Keybinds.REBIND_PAINT_COLOR_MODIFIER);

            loadButton.Initialize();
            IO.RENDERER.Add(loadButton);
            resaveButton.Initialize();
            IO.RENDERER.Add(resaveButton, EinsteinConfig.Render.DEFAULT_LAYER + 1);
            resaveButtonDisabledPlaceholder.Initialize();
            IO.RENDERER.Add(resaveButtonDisabledPlaceholder);
            IO.RENDERER.Add(bibiteNameText);
            IO.RENDERER.Add(bibiteVersionText);
            saveToButton.Initialize();
            IO.RENDERER.Add(saveToButton);

            autoArrangeButton.Initialize();
            IO.RENDERER.Add(autoArrangeButton);
            helpButton.Initialize();
            IO.RENDERER.Add(helpButton);
            zoomControls.Initialize();

            IO.KEYS.Subscribe(saveToBibite, EinsteinConfig.Keybinds.SAVE_TO_BIBITE);
            IO.KEYS.Subscribe(resaveToBibite, EinsteinConfig.Keybinds.SAVE_BIBITE);
            IO.KEYS.Subscribe(loadFromBibite, EinsteinConfig.Keybinds.LOAD_FROM_BIBITE);

            IO.FRAME_TIMER.Subscribe(checkForResize);

            editArea.Brain.MarkChangesAsSaved();
        }

        protected override void UninitializeMe()
        {
            editArea.Uninitialize();

            input.Uninitialize();
            output.Uninitialize();
            hidden.Uninitialize();
            editColorMenuCategory.Uninitialize();
            IO.RENDERER.Remove(paintColorDisplay);
            IO.KEYS.Unsubscribe(RebindColorShortcut1, (int)EinsteinConfig.Keybinds.SELECT_PAINT_COLORS[0] + (int)EinsteinConfig.Keybinds.REBIND_PAINT_COLOR_MODIFIER);
            IO.KEYS.Unsubscribe(RebindColorShortcut2, (int)EinsteinConfig.Keybinds.SELECT_PAINT_COLORS[1] + (int)EinsteinConfig.Keybinds.REBIND_PAINT_COLOR_MODIFIER);
            IO.KEYS.Unsubscribe(RebindColorShortcut3, (int)EinsteinConfig.Keybinds.SELECT_PAINT_COLORS[2] + (int)EinsteinConfig.Keybinds.REBIND_PAINT_COLOR_MODIFIER);
            IO.KEYS.Unsubscribe(RebindColorShortcut4, (int)EinsteinConfig.Keybinds.SELECT_PAINT_COLORS[3] + (int)EinsteinConfig.Keybinds.REBIND_PAINT_COLOR_MODIFIER);
            IO.KEYS.Unsubscribe(RebindColorShortcut5, (int)EinsteinConfig.Keybinds.SELECT_PAINT_COLORS[4] + (int)EinsteinConfig.Keybinds.REBIND_PAINT_COLOR_MODIFIER);
            IO.KEYS.Unsubscribe(RebindColorShortcut6, (int)EinsteinConfig.Keybinds.SELECT_PAINT_COLORS[5] + (int)EinsteinConfig.Keybinds.REBIND_PAINT_COLOR_MODIFIER);
            IO.KEYS.Unsubscribe(RebindColorShortcut7, (int)EinsteinConfig.Keybinds.SELECT_PAINT_COLORS[6] + (int)EinsteinConfig.Keybinds.REBIND_PAINT_COLOR_MODIFIER);
            IO.KEYS.Unsubscribe(RebindColorShortcut8, (int)EinsteinConfig.Keybinds.SELECT_PAINT_COLORS[7] + (int)EinsteinConfig.Keybinds.REBIND_PAINT_COLOR_MODIFIER);
            IO.KEYS.Unsubscribe(RebindColorShortcut9, (int)EinsteinConfig.Keybinds.SELECT_PAINT_COLORS[8] + (int)EinsteinConfig.Keybinds.REBIND_PAINT_COLOR_MODIFIER);
            IO.KEYS.Unsubscribe(RebindColorShortcut0, (int)EinsteinConfig.Keybinds.SELECT_PAINT_COLORS[9] + (int)EinsteinConfig.Keybinds.REBIND_PAINT_COLOR_MODIFIER);

            loadButton.Uninitialize();
            IO.RENDERER.Remove(loadButton);
            resaveButton.Uninitialize();
            IO.RENDERER.Remove(resaveButton);
            resaveButtonDisabledPlaceholder.Uninitialize();
            IO.RENDERER.Remove(resaveButtonDisabledPlaceholder);
            IO.RENDERER.Remove(bibiteNameText);
            IO.RENDERER.Remove(bibiteVersionText);
            saveToButton.Uninitialize();
            IO.RENDERER.Remove(saveToButton);

            autoArrangeButton.Uninitialize();
            IO.RENDERER.Remove(autoArrangeButton);
            IO.RENDERER.Remove(helpButton);
            zoomControls.Uninitialize();

            IO.KEYS.Unsubscribe(saveToBibite, EinsteinConfig.Keybinds.SAVE_TO_BIBITE);
            IO.KEYS.Unsubscribe(resaveToBibite, EinsteinConfig.Keybinds.SAVE_BIBITE);
            IO.KEYS.Unsubscribe(loadFromBibite, EinsteinConfig.Keybinds.LOAD_FROM_BIBITE);

            IO.FRAME_TIMER.Unsubscribe(checkForResize);
        }

        // ----------------------------------------------------------------
        //  Behavior
        // ----------------------------------------------------------------

        // TODO look at increasing performance of re-clicking the same drawable very quickly?

        // ----- Saving/Loading -----

        private string getResaveTargetFile()
        {
            return mostRecentSavedToFile ?? mostRecentLoadedToFile;
        }

        private void resaveToBibite()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }

            string targetFile = getResaveTargetFile();

            if (targetFile != null && saveToBibite(targetFile))
            {
                editArea.Brain.MarkChangesAsSaved();
                resaveButton.SetDisplaying(false);
            }
        }

        private void saveToBibite()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            // read file
            string filepath = IO.POPUPS.PromptForFile(getSavePath(), "Bibite Files|*.bb8",
                "Save to Bibite", "");
            if (filepath == "" || filepath == null)
            {
                // User cancelled
                return;
            }
            if (saveToBibite(filepath))
            {
                bibiteNameText.SetMessage(getBb8NameText());
                resaveButton.SetDisplaying(false);
            }
        }

        private bool saveToBibite(string filepath)
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }

            if (!File.Exists(filepath))
            {
                IO.POPUPS.ShowErrorPopup("Save Failed", "File not found.");
                return false;
            }
            string json = File.ReadAllText(filepath);

            // determine where to insert the brain
            int startIndex = json.IndexOf("\"brain\":") + "\"brain\":".Length;
            int endIndex = json.IndexOf("\"immuneSystem\":");
            if (startIndex < 0 || endIndex < 0)
            {
                IO.POPUPS.ShowErrorPopup("Save Failed", "File format is invalid: Cannot find an existing brain.");
                return false;
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
                return false;
            }
            BibiteVersion targetBibiteVersion;
            try
            {
                targetBibiteVersion = BibiteVersion.FromName(versionName);
            }
            catch (NoSuchVersionException e)
            {
                showVersionMismatchErrorPopup("Save Failed", e);
                return false;
            }

            BaseBrain brainToSave = editArea.Brain;
            // set diagram positions
            foreach (NeuronRenderable nr in editArea.NeuronRenderables)
            {
                int x = nr.NeuronDrawable.GetCenterX();
                int y = nr.NeuronDrawable.GetCenterY();
                JsonNeuron jn = (JsonNeuron)nr.Neuron;
                jn.DiagramX = x;
                jn.DiagramY = y;
            }

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
                    return false;
                }
                try
                {
                    brainToSave = bibiteVersion.CreateConvertedCopyOf(brainToSave, targetBibiteVersion);
                }
                catch (CannotConvertException e)
                {
                    IO.POPUPS.ShowErrorPopup("Unable to convert brain to "
                        + targetBibiteVersion.VERSION_NAME, e.Message);
                    return false;
                }
            }

            string brainJson = ((JsonBrain)brainToSave).GetSave(targetBibiteVersion);
            json = json.Substring(0, startIndex) + " " + brainJson + json.Substring(endIndex);
            File.WriteAllText(filepath, json);
            savePath = Path.GetDirectoryName(filepath);
            mostRecentSavedToFile = filepath;
            saveMessageText.Show();
            return true;
        }

        private void loadFromBibite()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }

            if (editArea.Brain.HasUnsavedChanges()
                && !IO.POPUPS.ShowYesNoPopup("Unsaved changes",
                "This brain has unsaved changes. Are you sure you want to load a new brain?"))
            {
                return;
            }

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
                    int indexOf1stInstance = 1 + Math.Max(json.IndexOf("\"" + originalDescription + "\""),
                        json.IndexOf("\"" + originalDescription + "-"));
                    int indexOf2ndInstance = 1 + Math.Max(json.IndexOf("\"" + originalDescription + "\"", indexOf1stInstance),
                        json.IndexOf("\"" + originalDescription + "-", indexOf1stInstance));
                    json = json.Substring(0, indexOf2ndInstance) +
                        newDescription +
                        json.Substring(indexOf2ndInstance + originalDescription.Length);
                    continue;
                }
                // TODO could catch InvalidDescriptionException and ask if they want the description automatically corrected
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
            editArea.LoadBrain(brain, newBibiteVersion);
            updateIONeuronsInMenu(); // depends on the brain being set in editArea.LoadBrain
            hidden.ResetNeuronOptionsTo(bibiteVersion.HiddenNeurons);

            loadPath = Path.GetDirectoryName(filepath);
            mostRecentLoadedToFile = filepath;
            mostRecentSavedToFile = null;
            brain.OnChangeFlagged += (_container) => {
                resaveButton.SetDisplaying(true);
            };
            resaveButton.SetDisplaying(false);
            bibiteNameText.SetMessage(getBb8NameText());
            bibiteVersionText.SetMessage(getBb8VersionText());
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
            editArea.AddNeuron(neuronRemovedFromMenu, true);
            /*
            if (editArea.PaintColor != null)
            {
                editArea.GetNROf(neuronRemovedFromMenu).NeuronDrawable.SetColorGroup((Color)editArea.PaintColor);
            }
            //*/
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
            editArea.isPainting = false;
        }
        private void onSelectOutputs()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            selected?.Button.Deselect();
            selected = output;
            editArea.isPainting = false;
        }
        private void onSelectHidden()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            selected?.Button.Deselect();
            selected = hidden;
            editArea.isPainting = false;
        }
        private void onSelectColors()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            selected?.Button.Deselect();
            selected = editColorMenuCategory;
            editArea.isPainting = true;
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
        private void onDeselectHidden()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            selected = null;
        }
        private void onDeselectColors()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            selected = null;
            editArea.isPainting = false;
        }

        // ----- other/misc -----

        private void RebindColorShortcut1()
        {
            editColorMenuCategory.RebindShortcut(editArea.PaintColor, 0);
        }
        private void RebindColorShortcut2()
        {
            editColorMenuCategory.RebindShortcut(editArea.PaintColor, 1);
        }
        private void RebindColorShortcut3()
        {
            editColorMenuCategory.RebindShortcut(editArea.PaintColor, 2);
        }
        private void RebindColorShortcut4()
        {
            editColorMenuCategory.RebindShortcut(editArea.PaintColor, 3);
        }
        private void RebindColorShortcut5()
        {
            editColorMenuCategory.RebindShortcut(editArea.PaintColor, 4);
        }
        private void RebindColorShortcut6()
        {
            editColorMenuCategory.RebindShortcut(editArea.PaintColor, 5);
        }
        private void RebindColorShortcut7()
        {
            editColorMenuCategory.RebindShortcut(editArea.PaintColor, 6);
        }
        private void RebindColorShortcut8()
        {
            editColorMenuCategory.RebindShortcut(editArea.PaintColor, 7);
        }
        private void RebindColorShortcut9()
        {
            editColorMenuCategory.RebindShortcut(editArea.PaintColor, 8);
        }
        private void RebindColorShortcut0()
        {
            editColorMenuCategory.RebindShortcut(editArea.PaintColor, 9);
        }


        private string getBb8VersionText()
        {
            return "Bibite Version: " + bibiteVersion.VERSION_NAME;
        }

        private string getBb8NameText()
        {
            return getResaveTargetFile() == null
                ? "Brain is not from a .bb8"
                : Path.GetFileName(getResaveTargetFile());
        }

        private void openHelp()
        {
            if (File.Exists(HELP_FILE_PATH))
            {
                Process.Start("notepad.exe", HELP_FILE_PATH);
            }
            else
            {
                IO.POPUPS.ShowErrorPopup("File Not Found",
                    "The help.txt file is missing. Try re-extracting Einstein from the zip file and running that fresh copy." +
                    "\n\nIf that doesn't work, and you're using the most recent version of Einstein, " +
                    "then please report this as a bug (see github page for how to).");
            }
        }

        public override bool CanClose()
        {
            if (editArea.Brain.HasUnsavedChanges())
            {
                return IO.POPUPS.ShowYesNoPopup("Unsaved changes", "This brain has unsaved changes. Are you sure you want to exit?");
            }
            return true;
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
                editColorMenuCategory.RepositionOptions();
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
