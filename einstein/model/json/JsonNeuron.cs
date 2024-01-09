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
        private float value;
        private float lastInput;
        private float lastOutput;

        public int DiagramX;
        public int DiagramY;

        public JsonNeuron(int index, NeuronType type, BibiteVersion bibiteVersion)
            : this(index, type, Enum.GetName(typeof(NeuronType), type), bibiteVersion) { }

        public JsonNeuron(int index, NeuronType type, string description, BibiteVersion bibiteVersion)
            : base(index, type, description, bibiteVersion)
        {
            Inov = 0;
            value = 0f;
            lastInput = 0f;
            lastOutput = 0f;
            ColorGroup = DEFAULT_COLOR_GROUP;
        }

        public JsonNeuron(JsonNeuron jsonNeuron)
            : base(jsonNeuron.Index, jsonNeuron.Type, jsonNeuron.Description, jsonNeuron.BibiteVersion)
        {
            Inov = jsonNeuron.Inov;
            value = jsonNeuron.value;
            lastInput = jsonNeuron.lastInput;
            lastOutput = jsonNeuron.lastOutput;
            DiagramX = jsonNeuron.DiagramX;
            DiagramY = jsonNeuron.DiagramY;
            ColorGroup = jsonNeuron.ColorGroup;
        }

        public JsonNeuron(JsonNeuron jsonNeuron, BibiteVersion bibiteVersion)
            : base(jsonNeuron.Index, jsonNeuron.Type, jsonNeuron.Description, bibiteVersion)
        {
            Inov = jsonNeuron.Inov;
            value = jsonNeuron.value;
            lastInput = jsonNeuron.lastInput;
            lastOutput = jsonNeuron.lastOutput;
            DiagramX = jsonNeuron.DiagramX;
            DiagramY = jsonNeuron.DiagramY;
            ColorGroup = jsonNeuron.ColorGroup;
        }

        public JsonNeuron(RawJsonFields jsonFields, BibiteVersion bibiteVersion) : base(bibiteVersion)
        {
            Type = (NeuronType)jsonFields.typeIndex;
            // skip TypeName
            Index = jsonFields.index;
            Inov = jsonFields.inov;
            Description = jsonFields.GetDescPiece(RawJsonFields.DescPiece.Description);
            value = jsonFields.value;
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
                this.typeName = jsonNeuron.Type.ToString();
                this.index = jsonNeuron.Index;
                this.inov = jsonNeuron.Inov;
                this.SetDescPiece(DescPiece.Description, jsonNeuron.Description);
                this.SetDescPiece(DescPiece.ColorHex, ColorTranslator.ToHtml(jsonNeuron.ColorGroup).Substring(1));
                this.value = jsonNeuron.value;
                this.lastInput = jsonNeuron.lastInput;
                this.lastOutput = jsonNeuron.lastOutput;
                jsonNeuron.BibiteVersion.SetNeuronDiagramPositionInRawJsonFields(
                    this, jsonNeuron.DiagramX, jsonNeuron.DiagramY);
            }

            public RawJsonFields(string json, int startIndex)
            {
                JsonParser parser = new JsonParser(json, startIndex);
                parser.startParsingNextLeafObj();
                this.typeIndex = parser.getNextValueInt("Type");
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

            public override string ToString()
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
