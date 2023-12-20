using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Einstein.model.json
{
    public class CustomNumberParser
    {
        private CustomNumberParser() { }
        
        public static int StringToInt(string str)
        {
            if (str.Length == 0)
            {
                throw new ArgumentException("Empty string cannot be parsed to an integer");
            }
            int output = 0;
            bool isNegative = str[0] == '-';
            if (isNegative)
            {
                if (str.Length == 1)
                {
                    throw new ArgumentException("Encountered non-digit character when parsing integer: '-'");
                }
                for (int i = 1 ; i < str.Length; i++)
                {
                    if (isDigit(str[i], out int d))
                    {
                        checked
                        {
                            output = output * 10 - d;
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Encountered non-digit character when parsing integer: '" + str[i] + "'");
                    }
                }
            }
            else
            {
                foreach (char c in str)
                {
                    if (isDigit(c, out int d))
                    {
                        checked
                        {
                            output = output * 10 + d;
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Encountered non-digit character when parsing integer: '" + c + "'");
                    }
                }
            }
            return output;
        }

        public static string IntToString(int value)
        {
            switch (value)
            {
                case 0:
                    return "0";
                case -2147483648:
                    return "-2147483648";
            }
            bool wasNegative = value < 0;
            value = Math.Abs(value);
            char[] output = new char[33];
            int lastCharIndex = 33;
            while (value > 0)
            {
                int valIntDiv10 = value / 10;
                int digit = value - valIntDiv10 * 10;
                value = valIntDiv10;
                output[--lastCharIndex] = toChar(digit);
            }
            if (wasNegative)
            {
                output[--lastCharIndex] = '-';
            }
            return new string(output, lastCharIndex, 33 - lastCharIndex);
        }

        public static string FloatToString(float value)
        {
            // TODO
            return "";
        }

        public static float StringToFloat(string str)
        {
            // TODO
            return 0f;
        }

        // ----- single-character stuff -----

        private const byte BYTE0 = (byte)'0';

        private static bool isDigit(char c)
        {
            return isDigit(c, out _);
        }
        private static bool isDigit(char c, out int d)
        {
            d = toDigit(c);
            return 0 <= d && d <= 9;
        }

        private static int toDigit(char c)
        {
            return ((byte)c) - BYTE0;
        }

        private static char toChar(int digit)
        {
            return (char)(BYTE0 + digit);
        }

        private static bool isDecimalPoint(char c)
        {
            return c == '.';
        }

    }
}
