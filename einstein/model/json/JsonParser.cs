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

        public JsonParser(string json, int index)
        {
            this.json = json;
            this.index = index;
            if (index < 0) { index = 0; }
        }

        public string getNextValue()
        {
            int indexLeft = json.IndexOf(':', index) + 1;
            if (indexLeft < 0)
            {
                throw new NoNextValueException("No ':' found after index " + index);
            }
            int indexRight = Math.Min(json.IndexOf(',', indexLeft),json.IndexOf('}', indexLeft));
            if (indexRight < 0) { indexRight = json.IndexOf('}'); }
            if (indexLeft < 0)
            {
                throw new NoNextValueException("No ',' or '}' found after index " + indexLeft);
            }
            int length = indexRight - indexLeft;
            string value = json.Substring(indexLeft, length).Trim('"');
            index = indexRight;
            return value;
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
