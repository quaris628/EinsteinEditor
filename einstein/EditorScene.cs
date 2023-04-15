using Einstein.config;
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

        private EditArea editArea;

        // TODO maybe refactor the input/output/hidden buttons into one renderable class
        private NeuronMenuCategory selected;
        private IONeuronMenuCategory input;
        private IONeuronMenuCategory output;
        private NeuronMenuCategory hidden;
        private Button loadButton;
        private Button saveButton;
        private SaveMessageText saveMessageText;
        private Button autoArrangeButton;
        private KeybindsInfoText infoText;
        private ZoomControls zoomControls;

        private string savePath;
        private string loadPath;
        private int prevWindowWidth;
        private int prevWindowHeight;

        public EditorScene(Scene prevScene) : base(prevScene, EinsteinConfig.COLOR_MODE.Background)
        {
            editArea = new EditArea(
                new JsonBrain(),
                moveNeuronIntoMenu);

            selected = null;
            NeuronMenuButton inputButton = new NeuronMenuButton(
                EinsteinConfig.PAD,
                EinsteinConfig.PAD,
                "Input Neurons",
                onSelectInputs,
                onDeselectInputs);
            input = new IONeuronMenuCategory(
                inputButton,
                generateInputNeurons(),
                moveNeuronIntoEditArea);

            NeuronMenuButton outputButton = new NeuronMenuButton(
                EinsteinConfig.PAD,
                2 * EinsteinConfig.PAD + NeuronMenuButton.HEIGHT,
                "Output Neurons",
                onSelectOutputs,
                onDeselectOutputs);
            output = new IONeuronMenuCategory(
                outputButton,
                generateOutputNeurons(),
                moveNeuronIntoEditArea);

            NeuronMenuButton hiddenButton = new NeuronMenuButton(
                EinsteinConfig.PAD,
                3 * EinsteinConfig.PAD + 2 * NeuronMenuButton.HEIGHT,
                "Hidden Neurons",
                onSelectAdd,
                onDeselectAdd);
            hidden = new HiddenNeuronMenuCategory(
                hiddenButton,
                generateHiddenNeurons(),
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
            saveMessageText = new SaveMessageText(
                EinsteinConfig.PAD + NeuronMenuButton.WIDTH / 2,
                7 * EinsteinConfig.PAD + 5 * NeuronMenuButton.HEIGHT);
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
            foreach (NeuronRenderable nr in editArea.NeuronRenderables)
            {
                int x = nr.NeuronDrawable.GetCenterX();
                int y = nr.NeuronDrawable.GetCenterY();
                ((JsonNeuron)nr.Neuron).SetInovXY(x, y);
            }
            string brainJson = ((JsonBrain)editArea.Brain).GetSave();
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
            string bibiteJson = File.ReadAllText(filepath);
            int startIndex = bibiteJson.IndexOf("\"brain\":") + "\"brain\":".Length;
            int endIndex = bibiteJson.IndexOf("\"immuneSystem\":");
            if (startIndex < 0 || endIndex < 0)
            {
                IO.POPUPS.ShowErrorPopup("Save Failed", "File format is invalid.");
                return;
            }
            bibiteJson = bibiteJson.Substring(0, startIndex) + " " + brainJson + bibiteJson.Substring(endIndex);
            File.WriteAllText(filepath, bibiteJson);
            savePath = Path.GetDirectoryName(filepath);
            saveMessageText.Show();
        }

        private void loadBrain()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            string filepath = IO.POPUPS.PromptForFile(getLoadPath(), "Bibite Files|*.bb8",
                "Load from Bibite", "");
            if (filepath == "") { return; }
            if (!File.Exists(filepath))
            {
                IO.POPUPS.ShowErrorPopup("Load Failed", "File not found.");
                return;
            }
            string json = File.ReadAllText(filepath);
            JsonBrain brain;
            bool autoFixNeuronDescriptions = false;
            while (true)
            {
                try
                {
                    brain = new JsonBrain(json, json.IndexOf("\"brain\":") + 8);
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
                    int indexOf2ndInstance = json.IndexOf(originalDescription, json.IndexOf(originalDescription));
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

            editArea.LoadBrain(brain);
            updateNeuronsInMenu();
            loadPath = Path.GetDirectoryName(filepath);
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

        private void updateNeuronsInMenu()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            input.ClearAllOptions();
            output.ClearAllOptions();
            foreach (BaseNeuron neuron in generateInputNeurons())
            {
                if (!editArea.Brain.ContainsNeuron(neuron))
                {
                    input.AddOption(neuron);
                }
            }
            foreach (BaseNeuron neuron in generateOutputNeurons())
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

        // ----- Generating neuron options -----

        public static ICollection<BaseNeuron> generateInputNeurons()
        {
            ICollection<BaseNeuron> inputs = new List<BaseNeuron>();
            for (int i = BibiteVersionConfig.INPUT_NODES_INDEX_MIN;
                i <= BibiteVersionConfig.INPUT_NODES_INDEX_MAX; i++)
            {
                inputs.Add(new JsonNeuron(
                    i,
                    NeuronType.Input,
                    BibiteVersionConfig.DESCRIPTIONS[i]));
            }
            return inputs;
        }

        public static ICollection<BaseNeuron> generateOutputNeurons()
        {
            ICollection<BaseNeuron> outputs = new List<BaseNeuron>();
            for (int i = BibiteVersionConfig.OUTPUT_NODES_INDEX_MIN;
                i <= BibiteVersionConfig.OUTPUT_NODES_INDEX_MAX; i++)
            {
                outputs.Add(new JsonNeuron(
                    i,
                    BibiteVersionConfig.GetOutputNeuronType(i),
                    BibiteVersionConfig.DESCRIPTIONS[i]));
            }
            return outputs;
        }

        public static ICollection<BaseNeuron> generateHiddenNeurons()
        {
            ICollection<BaseNeuron> hiddens = new List<BaseNeuron>();
            int index = BibiteVersionConfig.HIDDEN_NODES_INDEX_MAX
                - Enum.GetValues(typeof(NeuronType)).Length;
            foreach (NeuronType neuronType in Enum.GetValues(typeof(NeuronType)))
            {
                if (neuronType == NeuronType.Input) { continue; }
                hiddens.Add(new BaseNeuron(
                    index++,
                    neuronType));
            }
            return hiddens;
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


        // check if the window has been resized
        // if so, we probably need to reposition the menu buttons
        private void checkForResize()
        {
            if (!isInit) { throw new InvalidOperationException(this + " is not inited"); }
            if (IO.WINDOW.GetWidth() != prevWindowWidth)
            {
                input.RepositionOptions();
                output.RepositionOptions();
                hidden.RepositionOptions();
            }
            if (IO.WINDOW.GetHeight() != prevWindowHeight)
            {
                zoomControls.Reposition();
            }
        }

        public string LogDetailsForCrash()
        {
            string log = "Version = " + EinsteinConfig.VERSION;
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
            log += "hidden = " + hidden.LogDetailsForCrash();
            log += "\nsavePath = " + (savePath ?? "null");
            log += "\nloadPath = " + (loadPath ?? "null");
            log += "\nprevWindowWidth = " + prevWindowWidth;
            log += "\nisInit = " + isInit;
            return log;
        }
    }
}
