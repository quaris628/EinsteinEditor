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
            "        \"Value\": \"{5}\",\n" +
            "        \"LastInput\": {6},\n" +
            "        \"LastOutput\": {7}\n";

        // unused for now, but they're in the json so keep track of them just in case
        private string inov;
        private string value;
        private string lastInput;
        private string lastOutput;

        public JsonNeuron(int index, NeuronType type, string description)
            : base(index, type, description)
        {
            inov = "0";
            value = "0";
            lastInput = "0.0";
            lastOutput = "0.0";
        }

        public JsonNeuron(string json, int startIndex) : base()
        {
            JsonParser parser = new JsonParser(json, startIndex);
            Type = (NeuronType)int.Parse(parser.getNextValue());
            parser.getNextValue(); // skip TypeName
            Index = int.Parse(parser.getNextValue());
            inov = parser.getNextValue();
            Description = parser.getNextValue();
            value = parser.getNextValue();
            lastInput = parser.getNextValue();
            lastOutput = parser.getNextValue();
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
                lastOutput) + "      }";
        }
    }
}
