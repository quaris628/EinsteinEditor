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
        //   "en":true
        // }

        private const string JSON_FORMAT =
            "        \"Inov\": {0},\n" +
            "        \"NodeIn\": {1},\n" +
            "        \"NodeOut\": {2},\n" +
            "        \"Weight\": {3},\n" +
            "        \"En\": {4},\n" +
            "        \"en\": {5}\n";

        // unused for now
        private string inov;
        private string En;
        private string en;

        public JsonSynapse(JsonNeuron from, JsonNeuron to, float strength)
            : base(from, to, strength)
        {
            inov = "0";
            En = "true";
            en = "true";
        }

        public JsonSynapse(string json, int startIndex, JsonBrain brain)
            : base()
        {
            JsonParser parser = new JsonParser(json, startIndex);

            inov = parser.getNextValue();
            From = brain.GetNeuron(int.Parse(parser.getNextValue()));
            To = brain.GetNeuron(int.Parse(parser.getNextValue()));
            Strength = float.Parse(parser.getNextValue());
            En = parser.getNextValue();
            en = parser.getNextValue();
        }

        public override string GetSave()
        {
            return "{\n" + string.Format(JSON_FORMAT,
                inov,
                From.Index,
                To.Index,
                Strength,
                En,
                en) + "      }";
        }
    }
}
