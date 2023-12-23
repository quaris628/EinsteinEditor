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
            bool isNegative = value < 0;
            if (isNegative)
            {
                value = -value;
            }
            const int outputArrMaxSize = 12;
            char[] output = new char[outputArrMaxSize];
            int lastCharIndex = outputArrMaxSize;
            while (value > 0)
            {
                int valIntDiv10 = value / 10;
                int digit = value - valIntDiv10 * 10;
                value = valIntDiv10;

                output[--lastCharIndex] = toChar(digit);
            }
            if (isNegative)
            {
                output[--lastCharIndex] = '-';
            }
            return new string(output, lastCharIndex, outputArrMaxSize - lastCharIndex);
        }

        public static string FloatToString(float value)
        {
            switch (value)
            {
                case 0f: // also catches negative zero
                    return "0";
                case float.NaN:
                    throw new ArgumentException("Cannot parse NaN to string");
                case float.PositiveInfinity:
                case float.NegativeInfinity:
                    throw new ArgumentException("Cannot parse Infinity to string");
            }
            StringBuilder s = new StringBuilder();

            bool isNegative = value < 0;
            if (isNegative)
            {
                value = -value;
                s.Append('-');
            }

            int sigFigs = 0;
            // pseudo boolean, so I can rewrite 'if (nonzeroDigitAdded) { sigFigs++ }' as 'sigFigs += nonzeroDigitAdded'
            int nonzeroDigitAdded = 0;

            // invisible leading zeroes
            int digit = 0;
            int powerOf10 = 38;
            for (; powerOf10 > 0 && nonzeroDigitAdded == 0; powerOf10--)
            {
                popDigitFromFloat(value, powerOf10, out digit, ref nonzeroDigitAdded);
            }

            // digits before decimal point
            if (digit > 0)
            {
                powerOf10++;
            }
            for (; powerOf10 >= 0; powerOf10--)
            {
                value = popDigitFromFloat(value, powerOf10, out digit, ref nonzeroDigitAdded);
                s.Append(toChar(digit));
                sigFigs += nonzeroDigitAdded;
            }

            if (value > float.Epsilon)
            {
                // decimal point (if one is needed)
                s.Append('.');

                // digits after decimal point (if any)
                char[] decimalDigits = new char[38];
                int j = 0;
                for (; value > float.Epsilon && powerOf10 > -38 && sigFigs < 8; powerOf10--)
                {
                    value = popDigitFromFloat(value, powerOf10, out digit, ref nonzeroDigitAdded);
                    decimalDigits[j++] = toChar(digit);
                    sigFigs += nonzeroDigitAdded;
                }

                // trim any trailing zeroes
                for (; j > 0 && decimalDigits[j - 1] == '0'; j--) { }

                s.Append(decimalDigits, 0, j);
            }
            return s.ToString();
        }

        private static float popDigitFromFloat(float value, int powerOf10, out int digit, ref int isNonZero)
        {
            digit = (int)(value * DIGIT_POWERS_OF_10[38 + powerOf10]);
            if (0 < digit && digit < 10)
            {
                isNonZero = 1;
                return value - digit * DIGIT_POWERS_OF_10[38 - powerOf10];
            }
            else
            {
                digit = 0;
                return value;
            }
        }

        public static float StringToFloat(string str)
        {
            if (str.Length == 0)
            {
                throw new ArgumentException("Empty string cannot be parsed to a float");
            }
            float output = 0f;
            bool isNegative = str[0] == '-';
            if (isNegative)
            {
                if (str.Length == 1)
                {
                    throw new ArgumentException("Encountered non-digit character when parsing float: '-'");
                }
                int i = 1;
                for (; i < str.Length; i++)
                {
                    if (isDigit(str[i], out int d))
                    {
                        output = output * 10 - d;
                    }
                    else if (str[i] == '.')
                    {
                        i++;
                        break;
                    }
                    else
                    {
                        throw new ArgumentException("Encountered non-digit character when parsing float: '" + str[i] + "'");
                    }
                }
                float decBase = 0.1f;
                int maxCharsToParse = Math.Min(i + 40, str.Length);
                for (; i < maxCharsToParse; i++)
                {
                    if (isDigit(str[i], out int d))
                    {
                        output -= d * decBase;
                        decBase *= 0.1f;
                    }
                    else
                    {
                        throw new ArgumentException("Encountered non-digit character when parsing float: '" + str[i] + "'");
                    }
                }
            }
            else
            {
                int i = 0;
                for (; i < str.Length; i++)
                {
                    if (isDigit(str[i], out int d))
                    {
                        output = output * 10 + d;
                    }
                    else if (str[i] == '.')
                    {
                        i++;
                        break;
                    }
                    else
                    {
                        throw new ArgumentException("Encountered non-digit character when parsing float: '" + str[i] + "'");
                    }
                }
                float decBase = 0.1f;
                int maxCharsToParse = Math.Min(i + 40, str.Length);
                for (; i < maxCharsToParse; i++)
                {
                    if (isDigit(str[i], out int d))
                    {
                        output += d * decBase;
                        decBase *= 0.1f;
                    }
                    else
                    {
                        throw new ArgumentException("Encountered non-digit character when parsing float: '" + str[i] + "'");
                    }
                }
            }
            if (float.IsNaN(output) || float.IsInfinity(output))
            {
                throw new ArithmeticException("Parsed float evaulated to " + output);
            }
            return output;
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

        // ----- float values for powers of 10 -----

        private static readonly float[] DIGIT_POWERS_OF_10 = new float[]
        {
                1e+38f,
                1e+37f,
                1e+36f,
                1e+35f,
                1e+34f,
                1e+33f,
                1e+32f,
                1e+31f,
                1e+30f,
                1e+29f,
                1e+28f,
                1e+27f,
                1e+26f,
                1e+25f,
                1e+24f,
                1e+23f,
                1e+22f,
                1e+21f,
                1e+20f,
                1e+19f,
                1e+18f,
                1e+17f,
                1e+16f,
                1e+15f,
                1e+14f,
                1e+13f,
                1e+12f,
                1e+11f,
                1e+10f,
                1e+9f,
                1e+8f,
                1e+7f,
                1e+6f,
                1e+5f,
                1e+4f,
                1e+3f,
                1e+2f,
                1e+1f,
                1e+0f,
                1e-1f,
                1e-2f,
                1e-3f,
                1e-4f,
                1e-5f,
                1e-6f,
                1e-7f,
                1e-8f,
                1e-9f,
                1e-10f,
                1e-11f,
                1e-12f,
                1e-13f,
                1e-14f,
                1e-15f,
                1e-16f,
                1e-17f,
                1e-18f,
                1e-19f,
                1e-20f,
                1e-21f,
                1e-22f,
                1e-23f,
                1e-24f,
                1e-25f,
                1e-26f,
                1e-27f,
                1e-28f,
                1e-29f,
                1e-30f,
                1e-31f,
                1e-32f,
                1e-33f,
                1e-34f,
                1e-35f,
                1e-36f,
                1e-37f,
                1e-38f,
        };
    }
}
