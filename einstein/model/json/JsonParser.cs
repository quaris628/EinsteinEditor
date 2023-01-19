using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.model.json
{
    public class JsonParser
    {
        private string json;
        private int index;
        private int objEndIndex;

        public JsonParser(string json, int index)
        {
            this.json = json;
            this.index = index;
            if (index < 0) { this.index = 0; }
            
            // auto-fix a very specific corruption bug from previous versions
            int enBugIndex = json.IndexOf("\"en\": {activeViruses:");
            enBugIndex = enBugIndex < 0 ? json.IndexOf("\"en\":{activeViruses:") : enBugIndex;
            if (enBugIndex < 0) {
                int endIndex = json.IndexOf('}', enBugIndex);
                int startIndex = json.IndexOf(':', enBugIndex) + 1;
                this.json = json.Substring(0, startIndex) + " true" + json.Substring(endIndex);
            }
        }
        
        public void startParsingNextLeafObj() {
            index = json.IndexOf('{', index) + 1;
            objEndIndex = json.IndexOf('}', index);
        }

        public string getNextValue(string tag)
        {
            int indexLeft = json.IndexOf('"' + tag + "\":", index) + 1;
            if (indexLeft < 0)
            {
                throw new NoNextValueException("No value found for property '" + tag + "' after index " + index);
            }
            if (objEndIndex <= indexLeft)
            {
                throw new NoNextValueException("No value found for property '" + tag + "' between indices " + index + " and " + objEndIndex);
            }
            int indexRight = Math.Min(json.IndexOf(',', indexLeft),json.IndexOf('}', indexLeft));
            if (indexRight < 0) { indexRight = json.IndexOf('}'); }
            if (indexLeft < 0)
            {
                throw new NoNextValueException("No ',' or '}' found after index " + indexLeft);
            }
            int length = indexRight - indexLeft;
            string value = json.Substring(indexLeft, length);
            value = value.Replace("\"", "").Trim();
            index = indexRight;
            return value;
        }
        
        public string endParsingLeafObj() {
            index = objEndIndex;
            objEndIndex = -1;
        }

        public void parseArray(Action<int> eachItem)
        {
            int arrayStartIndex = json.IndexOf('[', index);
            int arrayEndIndex = json.IndexOf(']', arrayStartIndex);
            int itemStartIndex = json.IndexOf('{', arrayStartIndex);
            while (0 < itemStartIndex && itemStartIndex < arrayEndIndex)
            {
                eachItem.Invoke(itemStartIndex);
                itemStartIndex = json.IndexOf('{', itemStartIndex + 1);
            }
            index = arrayEndIndex;
        }
    }

    public class NoNextValueException : Exception
    {
        public NoNextValueException() : base() { }
        public NoNextValueException(string message) : base(message) { }
        public NoNextValueException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
