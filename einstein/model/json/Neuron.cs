using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.model.json
{
    public class Neuron : BaseNeuron
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

        public int Inov { protected set; get; }

        // unused for now, but they're in the json so keep track of them just in case
        private float value;
        private float lastInput;
        private float lastOutput;

        public Neuron(int index, NeuronType type, string description)
            : this(index, type, description, 0f, 0f, 0f) { }

        public Neuron(int index, NeuronType type, string description,
            float value, float lastInput, float lastOutput)
            : base(index, type, description)
        {
            Inov = 0;
            this.value = value;
            this.lastInput = lastInput;
            this.lastOutput = lastOutput;
        }

        public Neuron(Neuron jsonNeuron)
            : base(jsonNeuron.Index, jsonNeuron.Type, jsonNeuron.Description)
        {
            Inov = jsonNeuron.Inov;
            value = jsonNeuron.value;
            lastInput = jsonNeuron.lastInput;
            lastOutput = jsonNeuron.lastOutput;
        }

        public Neuron(string json, int startIndex) : base()
        {
            JsonParser parser = new JsonParser(json, startIndex);
            parser.startParsingNextLeafObj();

            Type = (NeuronType)parser.getNextValueInt("Type");
            // skip TypeName
            Index = parser.getNextValueInt("Index");
            Inov = parser.getNextValueInt("Inov");
            Description = parser.getNextValue("Desc");
            value = parser.getNextValueFloat("Value");
            lastInput = parser.getNextValueFloat("LastInput");
            lastOutput = parser.getNextValueFloat("LastOutput");
            
            parser.endParsingLeafObj();
        }

        public int GetInovX()
        {
            return Inov >> 16;
        }

        public int GetInovY()
        {
            return (Inov & 0xffff) - 32768;
        }

        public void SetInovXY(int x, int y)
        {
            Inov = (x << 16) | (y + 32768);
        }

        public override string GetSave()
        {
            return "{\n" + string.Format(JSON_FORMAT,
                (int)Type,
                Type.ToString(),
                Index,
                Inov,
                Description,
                value,
                lastInput,
                lastOutput)
                + "      }";
        }
    }
}
