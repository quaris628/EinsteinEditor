using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.model.json
{
    public class JsonNeuron : BaseNeuron
    {

        // Example:
        // {
        //   "Type":5,
        //   "TypeName":"ReLu",
        //   "Index":41,
        //   "Inov":0,
        //   "Desc":"PhereOut1",
        //   "Value":0.0,
        //   "LastInput":0.0,
        //   "LastOutput":0.0
        // }

        private const string JSON_FORMAT =
            "        \"Type\": {0},\n" +
            "        \"TypeName\": \"{1}\",\n" +
            "        \"Index\": {2},\n" +
            "        \"Inov\": {3},\n" +
            "        \"Desc\": \"{4}\",\n" +
            "        \"Value\": {5},\n" +
            "        \"LastInput\": {6},\n" +
            "        \"LastOutput\": {7}\n";

        // unused for now, but they're in the json so keep track of them just in case
        private int inov;
        private float value;
        private float lastInput;
        private float lastOutput;

        public JsonNeuron(int index, NeuronType type, string description)
            : base(index, type, description)
        {
            inov = 0;
            value = 0f;
            lastInput = 0f;
            lastOutput = 0f;
        }

        public JsonNeuron(string json, int startIndex) : base()
        {
            JsonParser parser = new JsonParser(json, startIndex);
            parser.startParsingNextLeafObj();

            Type = (NeuronType)parser.getNextValueInt("Type");
            // skip TypeName
            Index = parser.getNextValueInt("Index");
            inov = parser.getNextValueInt("Inov");
            Description = parser.getNextValue("Desc");
            value = parser.getNextValueFloat("Value");
            lastInput = parser.getNextValueFloat("LastInput");
            lastOutput = parser.getNextValueFloat("LastOutput");
            
            parser.endParsingLeafObj();
        }

        public override string GetSave()
        {
            return "{\n" + string.Format(JSON_FORMAT,
                (int)Type,
                Type.ToString(),
                Index,
                inov,
                Description,
                value,
                lastInput,
                lastOutput)
                + "      }";
        }
    }
}
