using System;
using System.IO;
using Einstein.model.json;

namespace Bibyte
{
   public class BibyteMain
   {
      public static void Main(string[] args)
      {
         Console.WriteLine("Building brain...");
         Brain brain = BrainCreator.GenerateBrain();
         Console.WriteLine("Saving brain...");
         SaveBrain(brain, BrainCreator.BB8_FILE_TO_SAVE_TO);
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
