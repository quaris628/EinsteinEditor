using Einstein.model;
using Einstein.model.json;
using Einstein.ui.editarea;
using Einstein.ui.menu;
using Einstein.ui.visibleElements;
using phi.control;
using phi.graphics.drawables;
using phi.io;
using System;
using System.Collections.Generic;
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
            + "\\AppData\\LocalLow\\The Bibites\\The Bibites";
        
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
        private KeybindsInfoText infoText;

        private string savePath;
        private string loadPath;
        private int prevWindowWidth;

        public EditorScene(Scene prevScene) : base(prevScene)
        {
            editArea = new EditArea(
                new JsonBrain(),
                moveNeuronIntoMenu);

            selected = null;
            NeuronMenuButton inputButton = new NeuronMenuButton(
                EinsteinPhiConfig.PAD,
                EinsteinPhiConfig.PAD,
                "Input Neurons",
                onSelectInputs,
                onDeselectInputs);
            input = new IONeuronMenuCategory(
                inputButton,
                generateInputNeurons(),
                moveNeuronIntoEditArea);

            NeuronMenuButton outputButton = new NeuronMenuButton(
                EinsteinPhiConfig.PAD,
                2 * EinsteinPhiConfig.PAD + NeuronMenuButton.HEIGHT,
                "Output Neurons",
                onSelectOutputs,
                onDeselectOutputs);
            output = new IONeuronMenuCategory(
                outputButton,
                generateOutputNeurons(),
                moveNeuronIntoEditArea);

            NeuronMenuButton hiddenButton = new NeuronMenuButton(
                EinsteinPhiConfig.PAD,
                3 * EinsteinPhiConfig.PAD + 2 * NeuronMenuButton.HEIGHT,
                "Hidden Neurons",
                onSelectAdd,
                onDeselectAdd);
            hidden = new HiddenNeuronMenuCategory(
                hiddenButton,
                generateHiddenNeurons(),
                createHiddenNeuronInEditArea);
            loadButton = new Button.ButtonBuilder(
                new ImageWrapper(NeuronMenuButton.UNSELECTED_IMAGE_PATH),
                EinsteinPhiConfig.PAD,
                5 * EinsteinPhiConfig.PAD + 3 * NeuronMenuButton.HEIGHT)
                .withText("Load from Bibite")
                .withOnClick(loadBrain)
                .Build();
            saveButton = new Button.ButtonBuilder(
                new ImageWrapper(NeuronMenuButton.UNSELECTED_IMAGE_PATH),
                EinsteinPhiConfig.PAD,
                6 * EinsteinPhiConfig.PAD + 4 * NeuronMenuButton.HEIGHT)
                .withText("Save to Bibite")
                .withOnClick(saveBrain)
                .Build();
            infoText = new KeybindsInfoText();

            savePath = null;
            loadPath = null;
            prevWindowWidth = EinsteinPhiConfig.Window.INITIAL_WIDTH;
        }

        // ----------------------------------------------------------------
        //  Initialize and Close
        // ----------------------------------------------------------------

        protected override void InitializeMe()
        {
            input.Initialize();
            output.Initialize();
            hidden.Initialize();
            loadButton.Initialize();
            saveButton.Initialize();
            IO.RENDERER.Add(loadButton);
            IO.RENDERER.Add(saveButton);
            infoText.Initialize();
            IO.FRAME_TIMER.Subscribe(checkForResize);
        }

        protected override void CloseMe()
        {
            IO.FRAME_TIMER.Unsubscribe(checkForResize);
        }

        // ----------------------------------------------------------------
        //  Behavior
        // ----------------------------------------------------------------

        // TODO look at increasing performance of re-clicking the same drawable very quickly?

        // ----- Saving/Loading -----

        private void saveBrain()
        {
            string brainJson = ((JsonBrain)editArea.Brain).GetSave();
            string filepath = IO.PromptForFile(getSavePath(), "Bibite Files|*.bb8",
                "Save to Bibite", "");
            if (filepath == "")
            {
                // User cancelled
                return;
            }
            if (!File.Exists(filepath))
            {
                IO.ShowErrorPopup("Save Failed", "File not found.");
                return;
            }
            string bibiteJson = File.ReadAllText(filepath);
            int startIndex = bibiteJson.IndexOf("\"brain\":") + "\"brain\":".Length;
            int endIndex = bibiteJson.IndexOf("\"immuneSystem\":");
            if (startIndex < 0 || endIndex < 0)
            {
                IO.ShowErrorPopup("Save Failed", "File format is invalid.");
                return;
            }
            bibiteJson = bibiteJson.Substring(0, startIndex) + " " + brainJson + bibiteJson.Substring(endIndex);
            File.WriteAllText(filepath, bibiteJson);
            savePath = Path.GetDirectoryName(filepath);
            IO.ShowPopup("Save Successful", ""); // TODO change to on-screen message in corner
        }

        private void loadBrain()
        {
            string filepath = IO.PromptForFile(getLoadPath(), "Bibite Files|*.bb8",
                "Load from Bibite", "");
            if (filepath == "") { return; }
            if (!File.Exists(filepath))
            {
                IO.ShowErrorPopup("Load Failed", "File not found.");
                return;
            }
            string json = File.ReadAllText(filepath);
            JsonBrain brain;
            try
            {
                brain = new JsonBrain(json, json.IndexOf("\"brain\":") + 8);
            }
            catch (NoNextValueException e)
            {
                IO.ShowErrorPopup("Load Failed",
                    "File format is invalid.\n\nDetails: " + e.Message);
                return;
            }
            catch (BrainException e)
            {
                IO.ShowErrorPopup("Load Failed",
                    "File format is correct but some values are invalid.\n\n" +
                    e.GetDisplayMessage(": "));
                return;
            }

            editArea.LoadBrain(brain);
            updateNeuronsInMenu();
            loadPath = Path.GetDirectoryName(filepath);
        }

        private string getSavePath()
        {
            if (savePath != null) { return savePath; }
            if (loadPath != null) { return loadPath; }
            return DEFAULT_SAVE_LOAD_FOLDER;
        }

        private string getLoadPath()
        {
            if (loadPath != null) { return loadPath; }
            if (savePath != null) { return savePath; }
            return DEFAULT_SAVE_LOAD_FOLDER;
        }

        private void updateNeuronsInMenu()
        {
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
            editArea.AddNeuron(neuronRemovedFromMenu);
        }
        private void createHiddenNeuronInEditArea(BaseNeuron hiddenNeuronToAdd)
        {
            editArea.CreateHiddenNeuron(hiddenNeuronToAdd.Type);
        }

        // ----- Generating neuron options -----

        private static ICollection<BaseNeuron> generateInputNeurons()
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

        private static ICollection<BaseNeuron> generateOutputNeurons()
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

        private static ICollection<BaseNeuron> generateHiddenNeurons()
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
            selected?.Button.Deselect();
            selected = input;
        }
        private void onSelectOutputs()
        {
            selected?.Button.Deselect();
            selected = output;
        }
        private void onSelectAdd()
        {
            selected?.Button.Deselect();
            selected = hidden;
        }

        private void onDeselectInputs() { selected = null; }
        private void onDeselectOutputs() { selected = null; }
        private void onDeselectAdd() { selected = null; }


        // check if the window has been resized
        // if so, we probably need to reposition the menu buttons
        private void checkForResize()
        {
            if (IO.WINDOW.GetWidth() != prevWindowWidth)
            {
                input.RepositionOptions();
                output.RepositionOptions();
                hidden.RepositionOptions();
            }
        }

        public string LogDetailsForCrash()
        {
            string log = "Version = " + EinsteinPhiConfig.VERSION;
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
            return log;
        }
    }
}
