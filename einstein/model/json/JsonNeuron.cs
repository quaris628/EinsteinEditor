using Einstein.config.bibiteVersions;
using LibraryFunctionReplacements;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.model.json
{
    public class JsonNeuron : BaseNeuron
    {
        public int Inov;

        // unused for now, but they're in the json so keep track of them just in case
        private float lastInput;
        private float lastOutput;

        public int DiagramX;
        public int DiagramY;

        public JsonNeuron(int index, NeuronType type, float bias, BibiteVersion bibiteVersion)
            : this(index, type, bias, Enum.GetName(typeof(NeuronType), type), bibiteVersion) { }

        public JsonNeuron(int index, NeuronType type, float bias, string description, BibiteVersion bibiteVersion)
            : base(index, type, bias, description, bibiteVersion)
        {
            Inov = 0;
            Value = 0f;
            lastInput = 0f;
            lastOutput = 0f;
            ColorGroup = DEFAULT_COLOR_GROUP;
        }

        public JsonNeuron(JsonNeuron jsonNeuron)
            : base(jsonNeuron.Index, jsonNeuron.Type, jsonNeuron.Bias, jsonNeuron.Description, jsonNeuron.BibiteVersion)
        {
            Inov = jsonNeuron.Inov;
            Value = jsonNeuron.Value;
            lastInput = jsonNeuron.lastInput;
            lastOutput = jsonNeuron.lastOutput;
            DiagramX = jsonNeuron.DiagramX;
            DiagramY = jsonNeuron.DiagramY;
            ColorGroup = jsonNeuron.ColorGroup;
        }

        public JsonNeuron(JsonNeuron jsonNeuron, BibiteVersion bibiteVersion)
            : base(jsonNeuron.Index, jsonNeuron.Type, jsonNeuron.Bias, jsonNeuron.Description, bibiteVersion)
        {
            Inov = jsonNeuron.Inov;
            Value = jsonNeuron.Value;
            lastInput = jsonNeuron.lastInput;
            lastOutput = jsonNeuron.lastOutput;
            DiagramX = jsonNeuron.DiagramX;
            DiagramY = jsonNeuron.DiagramY;
            ColorGroup = jsonNeuron.ColorGroup;
        }

        public JsonNeuron(RawJsonFields jsonFields, BibiteVersion bibiteVersion) : base(bibiteVersion)
        {
            Type = (NeuronType)jsonFields.typeIndex;
            Bias = jsonFields.bias;
            // skip TypeName
            Index = jsonFields.index;
            Inov = jsonFields.inov;
            Description = jsonFields.GetDescPiece(RawJsonFields.DescPiece.Description);
            Value = jsonFields.value;
            lastInput = jsonFields.lastInput;
            lastOutput = jsonFields.lastOutput;
            bibiteVersion.GetNeuronDiagramPositionFromRawJsonFields(jsonFields, ref DiagramX, ref DiagramY);
            ColorGroup = jsonFields.GetColorGroup();
        }

        public class RawJsonFields
        {
            public const char DESC_SPECIAL_DATA_DELIM = '-';

            public enum DescPiece
            {
                Description,
                DiagramPosX,
                DiagramPosY,
                ColorHex,
            }

            public int typeIndex;
            public float bias;
            public string typeName;
            public int index;
            public int inov;
            private string _desc;
            private string[] _splitDesc;
            public string rawDescription
            {
                get
                {
                    return _desc;
                }
                set
                {
                    _desc = value;
                    _splitDesc = _desc.Split(DESC_SPECIAL_DATA_DELIM);
                }
            }
            public float value;
            public float lastInput;
            public float lastOutput;

            public RawJsonFields(JsonNeuron jsonNeuron)
            {
                this.typeIndex = (int)jsonNeuron.Type;
                this.bias = jsonNeuron.Bias;
                this.typeName = jsonNeuron.Type.ToString();
                this.index = jsonNeuron.Index;
                this.inov = jsonNeuron.Inov;
                this.SetDescPiece(DescPiece.Description, jsonNeuron.Description);
                this.SetDescPiece(DescPiece.ColorHex, ColorTranslator.ToHtml(
                    Color.FromArgb(jsonNeuron.ColorGroup.ToArgb())).Substring(1));
                this.value = jsonNeuron.Value;
                this.lastInput = jsonNeuron.lastInput;
                this.lastOutput = jsonNeuron.lastOutput;
                jsonNeuron.BibiteVersion.SetNeuronDiagramPositionInRawJsonFields(
                    this, jsonNeuron.DiagramX, jsonNeuron.DiagramY);
            }

            public RawJsonFields(string json, int startIndex, BibiteVersion bibiteVersion)
            {
                JsonParser parser = new JsonParser(json, startIndex);
                parser.startParsingNextLeafObj();
                this.typeIndex = parser.getNextValueInt("Type");
                if (bibiteVersion.HasBiases())
                {
                    this.bias = parser.getNextValueFloat("baseActivation");
                }
                else
                {
                    this.bias = 0f;
                }
                this.typeName = parser.getNextValue("TypeName");
                this.index = parser.getNextValueInt("Index");
                this.inov = parser.getNextValueInt("Inov");
                this.rawDescription = parser.getNextValue("Desc");
                this.value = parser.getNextValueFloat("Value");
                this.lastInput = parser.getNextValueFloat("LastInput");
                this.lastOutput = parser.getNextValueFloat("LastOutput");
                parser.endParsingLeafObj();
            }

            public Color GetColorGroup()
            {
                if (HasDescPiece(DescPiece.ColorHex))
                {
                    string hex = GetDescPiece(DescPiece.ColorHex);
                    return ColorTranslator.FromHtml('#' + hex);
                }
                else
                {
                    return DEFAULT_COLOR_GROUP;
                }
            }

            public bool HasDescPiece(DescPiece piece)
            {
                return _splitDesc.Length > (int)piece;
            }

            public string GetDescPiece(DescPiece piece)
            {
                return _splitDesc[(int)piece];
            }

            public void SetDescPiece(DescPiece index, string piece)
            {
                if (_splitDesc == null)
                {
                    _splitDesc = new string[(int)index + 1];
                }
                else if (_splitDesc.Length <= (int)index)
                {
                    string[] newArr = new string[(int) index + 1];
                    Array.Copy(_splitDesc, newArr, _splitDesc.Length);
                    _splitDesc = newArr;
                }
                _splitDesc[(int)index] = piece;
                _desc = string.Join(DESC_SPECIAL_DATA_DELIM.ToString(), _splitDesc);
            }

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

            private const string JSON_FORMAT_BIASES =
                "{{\n" +
                "        \"Type\": {0},\n" +
                "        \"baseActivation\": {1},\n" +
                "        \"TypeName\": \"{2}\",\n" +
                "        \"Index\": {3},\n" +
                "        \"Inov\": {4},\n" +
                "        \"Desc\": \"{5}\",\n" +
                "        \"Value\": {6},\n" +
                "        \"LastInput\": {7},\n" +
                "        \"LastOutput\": {8}\n" +
                "      }}";

            private const string JSON_FORMAT =
                "{{\n" +
                "        \"Type\": {0},\n" +
                "        \"TypeName\": \"{1}\",\n" +
                "        \"Index\": {2},\n" +
                "        \"Inov\": {3},\n" +
                "        \"Desc\": \"{4}\",\n" +
                "        \"Value\": {5},\n" +
                "        \"LastInput\": {6},\n" +
                "        \"LastOutput\": {7}\n" +
                "      }}";

            public string ToString(BibiteVersion bibiteVersion)
            {
                if (bibiteVersion.HasBiases()) {
                    return string.Format(
                        CultureInfo.GetCultureInfo("en-US"),
                        JSON_FORMAT_BIASES,
                        typeIndex,
                        CustomNumberParser.FloatToString(bias),
                        typeName,
                        CustomNumberParser.IntToString(index),
                        CustomNumberParser.IntToString(inov),
                        rawDescription,
                        CustomNumberParser.FloatToString(value),
                        CustomNumberParser.FloatToString(lastInput),
                        CustomNumberParser.FloatToString(lastOutput));
                }
                else
                {
                    return string.Format(
                        CultureInfo.GetCultureInfo("en-US"),
                        JSON_FORMAT,
                        typeIndex,
                        typeName,
                        CustomNumberParser.IntToString(index),
                        CustomNumberParser.IntToString(inov),
                        rawDescription,
                        CustomNumberParser.FloatToString(value),
                        CustomNumberParser.FloatToString(lastInput),
                        CustomNumberParser.FloatToString(lastOutput));
                }
            }
        }
    }
}
