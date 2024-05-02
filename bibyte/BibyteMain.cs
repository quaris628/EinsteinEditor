using System;
using System.Diagnostics;
using System.IO;
using bibyte.functional.brains;
using Bibyte.functional;
using Bibyte.functional.background;
using Bibyte.neural;
using Einstein;
using Einstein.config;
using Einstein.config.bibiteVersions;
using Einstein.model.json;
using phi;

namespace Bibyte
{
    public class BibyteMain
    {

        private static string FILE_WITH_SAVE_FILE_PATH = "saveFilePath.txt";

        [STAThread] // must be specified in order to call phi
        public static void Main(string[] args)
        {
            // get the file we're saving to
            if (!File.Exists(FILE_WITH_SAVE_FILE_PATH))
            {
                File.CreateText(FILE_WITH_SAVE_FILE_PATH);
                Process.Start("notepad.exe", FILE_WITH_SAVE_FILE_PATH);
                Console.WriteLine("Please specify the bibite file to save to.\n"
                    + "Enter the filepath into this text file, then save the text file and run Bibyte again.\n");
                return;
            }
            string saveFilePath = File.ReadAllText(FILE_WITH_SAVE_FILE_PATH);
            if (!File.Exists(saveFilePath))
            {
                Process.Start("notepad.exe", FILE_WITH_SAVE_FILE_PATH);
                Console.WriteLine("This bibite file does not exist.\n"
                    + "Look for errors or try a different filepath, then save the text file and run Bibyte again.\n");
            }

            // uncomment the section that corresponds to the type of programming you want to use

            // use functional programming
            //*
            Console.WriteLine("Building functionally programmed brain");
            Console.WriteLine("Creating brain...");
            JsonBrain brain = FunctionalBrainBuilder.Build(BrainToGenerate.BRAIN_TO_GENERATE, out BibiteVersion bibiteVersion);
            Console.WriteLine("Saving brain...");
            SaveBrain(brain, saveFilePath, bibiteVersion);
            //*/

            // use neural programming
            /*
            Console.WriteLine("Building neurally programmed brain");
            Console.WriteLine("Creating brain...");
            NeuralBrainCreator.CreateBrain();
            Console.WriteLine("Saving brain...");
            SaveBrain(NeuralBackgroundBrainBuilder.GetBrain(), saveFilePath, NeuronFactory.GetBibiteVersion());
            //*/

            PhiMain.Main(new EditorScene(null, saveFilePath), new EinsteinConfig());
        }

        public static void SaveBrain(JsonBrain brain, string filepath, BibiteVersion bibiteVersion)
        {
            string brainJson = brain.GetSave(bibiteVersion);
            if (!File.Exists(filepath))
            {
                Console.WriteLine("Error, file does not exist: '" + filepath + "'");
                return;
            }
            string bibiteJson = File.ReadAllText(filepath);
            int startIndex = bibiteJson.IndexOf("\"brain\":") + "\"brain\":".Length;
            int endIndex = bibiteJson.IndexOf("\"immuneSystem\":");
            if (startIndex < 0 || endIndex < 0)
            {
                Console.WriteLine("Error, file format is invalid. Be sure you're saving to a .bb8 file. Template bibites aren't supported.");
                return;
            }
            bibiteJson = bibiteJson.Substring(0, startIndex) + " " + brainJson + bibiteJson.Substring(endIndex);
            File.WriteAllText(filepath, bibiteJson);
            Console.WriteLine("Brain saved");
        }
    }
}
