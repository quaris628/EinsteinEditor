using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.model.json
{
    public class JsonSynapse : BaseSynapse
    {

        // Example:
        // {
        //   "Inov":0,
        //   "NodeIn":0,
        //   "NodeOut":48,
        //   "Weight":1.0,
        //   "En":true,
        // }

        private const string JSON_FORMAT =
            "        \"Inov\": {0},\n" +
            "        \"NodeIn\": {1},\n" +
            "        \"NodeOut\": {2},\n" +
            "        \"Weight\": {3},\n" +
            "        \"En\": {4}\n";

        // unused for now, but keep track of between loading and saving just in case
        private double inov;
        private bool En;

        public JsonSynapse(JsonNeuron from, JsonNeuron to, float strength)
            : base(from, to, strength)
        {
            inov = 0;
            En = true;
        }

        public JsonSynapse(string json, int startIndex, JsonBrain brain)
            : base()
        {
            JsonParser parser = new JsonParser(json, startIndex);
            parser.startParsingNextLeafObj();
            
            inov = parser.getNextValueDouble("Inov");

            int fromIndex = parser.getNextValueInt("NodeIn");
            if (!brain.ContainsNeuron(fromIndex))
            {
                throw new DanglingSynapseException("Brain does not contain a neuron with index " + fromIndex);
            }
            From = brain.GetNeuron(fromIndex);
            
            int toIndex = parser.getNextValueInt("NodeOut");
            if (!brain.ContainsNeuron(toIndex))
            {
                throw new DanglingSynapseException("Brain does not contain a neuron with index " + toIndex);
            }
            To = brain.GetNeuron(toIndex);
            
            Strength = parser.getNextValueFloat("Weight");

            En = parser.getNextValueBool("En");

            parser.endParsingLeafObj();
        }

        // Created as a workaround to an in-game bug
        // Only use this if you know what you're doing
        // Saves this synapse but with the specified neuron index values
        public string GetSave(int fromIndex, int toIndex)
        {
            return "{\n" + string.Format(JSON_FORMAT,
                inov,
                fromIndex,
                toIndex,
                Strength,
                En ? "true" : "false")
                + "      }";
        }

        public override string GetSave()
        {
            return "{\n" + string.Format(JSON_FORMAT,
                inov,
                From.Index,
                To.Index,
                Strength,
                En ? "true" : "false")
                + "      }";
        } 
    }
    public class DanglingSynapseException : JsonParsingException
    {
        public DanglingSynapseException() : base() { }
        public DanglingSynapseException(string message) : base(message) { }
        public DanglingSynapseException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
