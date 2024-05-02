using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryFunctionReplacements
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
            return FloatToString(value, 6, int.MaxValue);
        }

        public static string FloatToString(float value, int maxSigFigs, int maxDecimals)
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
            int powerOf10 = 38;
            for (; powerOf10 > 0 && nonzeroDigitAdded == 0; powerOf10--)
            {
                popDigitFromFloat(value, powerOf10, out _, ref nonzeroDigitAdded);
            }

            // digits before decimal point
            char[] wholePartDigits = new char[39];
            int digit;
            powerOf10 += nonzeroDigitAdded;
            int wholePartDigitsStartIndex = 38 - powerOf10;
            for (; powerOf10 >= 0; powerOf10--)
            {
                value -= popDigitFromFloat(value, powerOf10, out digit, ref nonzeroDigitAdded);
                wholePartDigits[38 - powerOf10] = toChar(digit);
                sigFigs += nonzeroDigitAdded;
            }

            if (value > float.Epsilon)
            {
                
                // digits after decimal point (if any)
                char[] decimalDigits = new char[39];
                int j = 0;
                for (; value > float.Epsilon && powerOf10 > -38 && sigFigs < maxSigFigs && j < maxDecimals; powerOf10--)
                {
                    value -= popDigitFromFloat(value, powerOf10, out digit, ref nonzeroDigitAdded);
                    decimalDigits[j++] = toChar(digit);
                    sigFigs += nonzeroDigitAdded;
                }
                if (powerOf10 > -38 && value >= 5 * DIGIT_POWERS_OF_10[38 - powerOf10])
                {
                    // round up the last digit
                    if (j == 0)
                    {
                        roundUpWholePartDigits(wholePartDigits, 38, ref wholePartDigitsStartIndex);
                    }
                    else
                    {
                        roundUpDecimalDigits(wholePartDigits, decimalDigits, j - 1, ref wholePartDigitsStartIndex);
                    }                    
                }

                // trim any trailing zeroes
                for (; j > 0 && decimalDigits[j - 1] == '0'; j--) { }

                // assemble the string
                s.Append(wholePartDigits, wholePartDigitsStartIndex, 39 - wholePartDigitsStartIndex);
                if (j > 0)
                {
                    s.Append('.');
                    s.Append(decimalDigits, 0, j);
                }
            }
            else
            {
                s.Append(wholePartDigits, wholePartDigitsStartIndex, 39 - wholePartDigitsStartIndex);
            }
            return s.ToString();
        }

        private static float popDigitFromFloat(float value, int powerOf10, out int digit, ref int isNonZero)
        {
            digit = (int)(value * DIGIT_POWERS_OF_10[38 + powerOf10]);
            if (0 < digit)
            {
                isNonZero = 1;
                return digit * DIGIT_POWERS_OF_10[38 - powerOf10];
            }
            else
            {
                digit = 0;
                return 0;
            }
        }

        private static void roundUpDecimalDigits(char[] wholePartDigits, char[] decimalDigits, int digitIndex, ref int wholePartDigitsStartIndex)
        {
            char digit = decimalDigits[digitIndex];
            switch (digit)
            {
                // skip over the decimal character and start rounding the whole-part digits
                case '.':
                    roundUpWholePartDigits(wholePartDigits, 38, ref wholePartDigitsStartIndex);
                    break;
                // if the last digit was a 9, round up the digit before that, etc...
                case '9':
                    decimalDigits[digitIndex] = '0';
                    roundUpDecimalDigits(wholePartDigits, decimalDigits, digitIndex - 1, ref wholePartDigitsStartIndex);
                    break;
                default:
                    decimalDigits[digitIndex] = (char)(digit + 1);
                    break;
            }
        }

        private static void roundUpWholePartDigits(char[] wholePartDigits, int digitIndex, ref int wholePartDigitsStartIndex)
        {
            int digit = wholePartDigits[digitIndex];
            switch (digit)
            {
                // if the last digit was a 9 and has been rounded up, add one more digit to the front
                case '\0':
                    wholePartDigits[digitIndex] = '0';
                    wholePartDigitsStartIndex++;
                    break;
                // if the last digit was a 9, round up the digit before that, etc...
                case '9':
                    wholePartDigits[digitIndex] = '0';
                    roundUpWholePartDigits(wholePartDigits, digitIndex - 1, ref wholePartDigitsStartIndex);
                    break;
                default:
                    wholePartDigits[digitIndex] = (char)(digit + 1);
                    break;
            }
        }

        public static float StringToFloat(string str)
        {
            switch (str)
            {
                case "":
                    throw new ArgumentException("Empty string cannot be parsed to a float");
                case ".":
                case "-":
                    throw new ArgumentException("Encountered non-digit character when parsing float: '" + str + "'");
            }

            float output = 0f;
            bool isNegative = str[0] == '-';
            if (isNegative)
            {
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
                    else if (str[i] == 'e' || str[i] == 'E')
                    {
                        int exponent = StringToInt(str.Substring(i + 1));
                        try
                        {
                            output *= DIGIT_POWERS_OF_10[38 - exponent];
                        }
                        catch (IndexOutOfRangeException)
                        {
                            throw new ArithmeticException($"Exponent '{exponent}' in '{str}' exceeds maximum or minimum bounds for single-precision floating point numbers");
                        }
                        i = str.Length; // stop second for loop from continuing
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
                    else if (str[i] == 'e' || str[i] == 'E')
                    {
                        int exponent = StringToInt(str.Substring(i + 1));
                        try
                        {
                            output *= DIGIT_POWERS_OF_10[38 - exponent];
                        }
                        catch (IndexOutOfRangeException)
                        {
                            throw new ArithmeticException($"Exponent '{exponent}' in '{str}' exceeds maximum or minimum bounds for single-precision floating point numbers");
                        }
                        break;
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
                    else if (str[i] == 'e' || str[i] == 'E')
                    {
                        int exponent = StringToInt(str.Substring(i + 1));
                        try
                        {
                            output *= DIGIT_POWERS_OF_10[38 - exponent];
                        }
                        catch (IndexOutOfRangeException)
                        {
                            throw new ArithmeticException($"Exponent '{exponent}' in '{str}' exceeds maximum or minimum bounds for single-precision floating point numbers");
                        }
                        i = str.Length; // stop second for loop from continuing
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
                    else if (str[i] == 'e' || str[i] == 'E')
                    {
                        int exponent = StringToInt(str.Substring(i + 1));
                        try
                        {
                            output *= DIGIT_POWERS_OF_10[38 - exponent];
                        }
                        catch (IndexOutOfRangeException)
                        {
                            throw new ArithmeticException($"Exponent '{exponent}' in '{str}' exceeds maximum or minimum bounds for single-precision floating point numbers");
                        }
                        break;
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

        private const byte CHAR_0_AS_BYTE = (byte)'0';

        private static bool isDigit(char c, out int d)
        {
            d = toDigit(c);
            return 0 <= d && d <= 9;
        }

        private static int toDigit(char c)
        {
            return ((byte)c) - CHAR_0_AS_BYTE;
        }

        private static char toChar(int digit)
        {
            return (char)(CHAR_0_AS_BYTE + digit);
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
                1e+0f, // index 38
                1e-1f,
                1e-2f + 1e-7f, // band-aid fix to mysterious bug with 100f not getting the 1 digit detected sometimes (but not always)
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
