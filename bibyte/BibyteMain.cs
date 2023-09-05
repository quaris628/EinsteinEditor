using System;
using System.IO;
using Bibyte.functional;
using Bibyte.neural;
using Einstein.model.json;

namespace Bibyte
{
    public class BibyteMain
    {
        public static void Main(string[] args)
        {
            // uncomment the section that corresponds to the type of programming you want to use

            // use functional programming
            //*
            Console.WriteLine("Building functionally programmed brain");
            Console.WriteLine("Creating brain...");
            Brain brain = FunctionalBackgroundBrainBuilder.Build(new MinimalBrain());
            Console.WriteLine("Saving brain...");
            SaveBrain(brain, NeuralBrainCreator.BB8_FILE_TO_SAVE_TO);
            //*/

            // use neural programming
            /*
            Console.WriteLine("Building neurally programmed brain");
            Console.WriteLine("Creating brain...");
            NeuralBrainCreator.CreateBrain();
            Console.WriteLine("Saving brain...");
            SaveBrain(NeuralBackgroundBrainBuilder.GetBrain(), NeuralBrainCreator.BB8_FILE_TO_SAVE_TO);
            //*/
        }

        public static void SaveBrain(Brain brain, string filepath)
        {
            string brainJson = brain.GetSave();
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
