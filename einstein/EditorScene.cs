using Einstein.model;
using Einstein.model.json;
using Einstein.ui.editarea;
using Einstein.ui.menu;
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

        private const string SAVE_LOAD_BUTTON_IMAGE = EinsteinPhiConfig.RES_DIR + "SmallButtonBackground.png";
        private const int SAVE_LOAD_BUTTON_WIDTH = 69;
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

        private string savePath;
        private string loadPath;
        private int prevWindowWidth;

        public EditorScene(Scene prevScene) : base(prevScene, new ImageWrapper(EinsteinPhiConfig.Render.DEFAULT_BACKGROUND))
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
                new ImageWrapper(SAVE_LOAD_BUTTON_IMAGE),
                EinsteinPhiConfig.PAD,
                4 * EinsteinPhiConfig.PAD + 3 * NeuronMenuButton.HEIGHT)
                .withText("Load")
                .withOnClick(loadBrain)
                .Build();
            saveButton = new Button.ButtonBuilder(
                new ImageWrapper(SAVE_LOAD_BUTTON_IMAGE),
                2 * EinsteinPhiConfig.PAD + SAVE_LOAD_BUTTON_WIDTH,
                4 * EinsteinPhiConfig.PAD + 3 * NeuronMenuButton.HEIGHT)
                .withText("Save")
                .withOnClick(saveBrain)
                .Build();

            savePath = null;
            loadPath = null;
            prevWindowWidth = EinsteinPhiConfig.Window.WIDTH;
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
            string filepath = IO.PromptForFile(getSavePath(), "", ".bb8");
            if (filepath == "") { return; }
            if (!File.Exists(filepath)) { throw new FileNotFoundException(filepath); }

            string bibiteJson = File.ReadAllText(filepath);
            int startIndex = bibiteJson.IndexOf("\"brain\":") + "\"brain\":".Length;
            int endIndex = bibiteJson.IndexOf("\"immuneSystem\":");
            bibiteJson = bibiteJson.Substring(0, startIndex) + brainJson + bibiteJson.Substring(endIndex);
            File.WriteAllText(filepath, bibiteJson);
            savePath = filepath.Substring(0, filepath.LastIndexOf("\\"));
        }

        private void loadBrain()
        {
            string filepath = IO.PromptForFile(getLoadPath(), "", ".bb8");
            if (filepath == "") { return; }
            if (!File.Exists(filepath)) { throw new FileNotFoundException(filepath); }
            string json = File.ReadAllText(filepath);
            JsonBrain brain = new JsonBrain(json, json.IndexOf("\"brain\":"));
            editArea.LoadBrain(brain);
            updateNeuronsInMenu();
            loadPath = filepath.Substring(0, filepath.LastIndexOf("\\"));
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
            for (int i = VersionConfig.INPUT_NODES_INDEX_MIN;
                i <= VersionConfig.INPUT_NODES_INDEX_MAX; i++)
            {
                inputs.Add(new JsonNeuron(
                    i,
                    NeuronType.Input,
                    VersionConfig.DESCRIPTIONS[i]));
            }
            return inputs;
        }

        private static ICollection<BaseNeuron> generateOutputNeurons()
        {
            ICollection<BaseNeuron> outputs = new List<BaseNeuron>();
            for (int i = VersionConfig.OUTPUT_NODES_INDEX_MIN;
                i <= VersionConfig.OUTPUT_NODES_INDEX_MAX; i++)
            {
                outputs.Add(new JsonNeuron(
                    i,
                    VersionConfig.GetOutputNeuronType(i),
                    VersionConfig.DESCRIPTIONS[i]));
            }
            return outputs;
        }

        private static ICollection<BaseNeuron> generateHiddenNeurons()
        {
            ICollection<BaseNeuron> hiddens = new List<BaseNeuron>();
            int index = VersionConfig.HIDDEN_NODES_INDEX_MAX
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
            log += "\nsavePath = " + (savePath != null ? savePath : "null");
            log += "\nloadPath = " + (loadPath != null ? loadPath : "null");
            log += "\nprevWindowWidth = " + prevWindowWidth;
            return log;
        }
    }
}
