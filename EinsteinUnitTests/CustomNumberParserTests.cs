using LibraryFunctionReplacements;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace LibraryFunctionReplacementsUnitTests
{
    [TestClass]
    public class CustomNumberParserTests
    {
        #region Test case lists

        // ----- Integers -----

        private static readonly (string, int)[] NORMAL_INT_TESTS = new (string, int)[]
        {
            ("0", 0),
            ("1", 1),
            ("-1", -1),
            ("9", 9),
            ("10", 10),
            ("11", 11),
            ("20", 20),
            ("99", 99),
            ("100", 100),
            ("101", 101),
            ("-99", -99),
            ("-100", -100),
            ("-101", -101),
            ("1378314", 1378314),
            ("-4968451", -4968451),
            ("2147483646", 2147483646),
            ("2147483647", 2147483647),
            ("-2147483646", -2147483646),
            ("-2147483647", -2147483647),
            ("-2147483648", -2147483648),
        };

        private static readonly string[] INVALID_STRING_NOT_AN_INT_TESTS = new string[]
        {
            "",
            " ",
            "-",
            ".",
            "?",
            "A",
            "4A",
            "A4",
            "0.5",
            ".5",
            "-3.5",
            "-35-",
            "1041,654,131.00",
            "543'246'434,000",
            "5 e10",
            "1.1.1",
            "-0.0000000004656612873077392578120",
        };

        private static readonly string[] INVALID_ARITHMETIC_OVERFLOW_INT_TESTS = new string[]
        {
            "2147483648",
            "-2147483649",
            "2147483648.0000000004656612873077392578125",
        };

        // ----- Floats -----

        private static readonly (string str, float num, float epsilon)[] NORMAL_FLOAT_TESTS
            = new (string str, float num, float epsilon)[]
        {
            ("0.3", 0.3f, float.Epsilon),
            ("0.25", 0.25f, float.Epsilon),
            ("-3.2", -3.2f, float.Epsilon),
            ("0", 0, float.Epsilon),
            ("1", 1, float.Epsilon),
            ("-1", -1, float.Epsilon),
            ("9", 9, float.Epsilon),
            ("10", 10, float.Epsilon),
            ("11", 11, float.Epsilon),
            ("99", 99, float.Epsilon),
            ("100", 100, float.Epsilon),
            ("101", 101, float.Epsilon),
            ("-99", -99, float.Epsilon),
            ("-100", -100, float.Epsilon),
            ("-101", -101, float.Epsilon),
            ("1378314", 1378314, float.Epsilon),
            ("-4968451", -4968451, float.Epsilon),
            ("2147483648", 2147483648, 1000f),
            ("-2147483648", -2147483648, 1000f),
            ("2147.4836", 2147.4836f, 0.001f),
            ("-2147.4836", -2147.4836f, 0.001f),
            ("-0.0000000004656612873077392578120", -0.0000000004656612873077392578120f, 0.0000000000000001f),
            ("2147483648.0000000004656612873077392578125", 2147483648f, 1000f),
            ("340282356779000000000000000000000000000", 340282356779000000000000000000000000000f, 10000000000000000000000000000f),
            ("-340282356779000000000000000000000000000", -340282356779000000000000000000000000000f, 10000000000000000000000000000f),
            ("123456780000000000000000000000000000", 123456780000000000000000000000000000f, 10000000000000000000000000000f),
            ("-123456780000000000000000000000000000", -123456780000000000000000000000000000f, 10000000000000000000000000000f),
            ("123456780000000000000000000000", 123456780000000000000000000000f, 10000000000000000000000f),
            ("-123456780000000000000000000000", -123456780000000000000000000000f, 10000000000000000000000f),
            ("123456780000000000000000", 123456780000000000000000f, 10000000000000000f),
            ("-123456780000000000000000", -123456780000000000000000f, 10000000000000000f),
            ("123456780000000000", 123456780000000000f, 10000000000f),
            ("-123456780000000000", -123456780000000000f, 10000000000f),
            ("123456780000", 123456780000f, 10000f),
            ("-123456780000", -123456780000f, 10000f),
            ("123456.78", 123456.78f, 0.01f),
            ("-123456.78", -123456.78f, 0.01f),
            ("0.12345678", 0.12345678f, 0.000001f),
            ("-0.12345678", -0.12345678f, 0.000001f),
            ("0.00000012345678", 0.00000012345678f, 0.0000000000001f),
            ("-0.00000012345678", -0.00000012345678f, 0.0000000000001f),
            ("0.00000000000012345678", 0.00000000000012345678f, 0.0000000000000000001f),
            ("-0.00000000000012345678", -0.00000000000012345678f, 0.0000000000000000001f),
            ("0.00000000000000000012345678", 0.00000000000000000012345678f, 0.0000000000000000000000001f),
            ("-0.00000000000000000012345678", -0.00000000000000000012345678f, 0.0000000000000000000000001f),
            ("0.00000000000000000000000012345678", 0.00000000000000000000000012345678f, 0.0000000000000000000000000000001f),
            ("-0.00000000000000000000000012345678", -0.00000000000000000000000012345678f, 0.0000000000000000000000000000001f),
        };

        private static readonly string[] INVALID_STRING_NOT_A_FLOAT_TESTS = new string[]
        {
            "",
            " ",
            "-",
            ".",
            "?",
            "A",
            "4A",
            "A4",
            "-35-",
            "1041,654,131.00",
            "543'246'434,000",
            "5 e10",
            "1.1.1",
        };

        private static readonly string[] INVALID_ARITHMETIC_OVERFLOW_FLOAT_TESTS = new string[]
        {
            "3402823567800000000000000000000000000000",
            "-3402823567800000000000000000000000000000",
            "4402823567800000000000000000000000000000",
            "-4402823567800000000000000000000000000000",
        };


        #endregion Test case lists

        #region Integers

        [TestMethod]
        public void IntToStringNormal()
        {
            foreach ((string str, int num) in NORMAL_INT_TESTS)
            {
                Assert.AreEqual(str, CustomNumberParser.IntToString(num));
            }
        }

        [TestMethod]
        public void StringToIntNormal()
        {
            foreach ((string str, int num) in NORMAL_INT_TESTS)
            {
                Assert.AreEqual(num, CustomNumberParser.StringToInt(str));
            }
        }

        [TestMethod]
        public void StringToIntNormal_LeadingZeros()
        {
            Assert.AreEqual(0, CustomNumberParser.StringToInt("000"));
            Assert.AreEqual(3, CustomNumberParser.StringToInt("003"));
            Assert.AreEqual(3, CustomNumberParser.StringToInt("00000000000000000000000000000000000000000003"));
        }

        [TestMethod]
        public void StringToIntNormal_NegativeZero()
        {
            Assert.AreEqual(0, CustomNumberParser.StringToInt("-0"));
        }

        [TestMethod]
        public void StringToIntInvalid_NotANumber_ArgumentException()
        {
            foreach (string test in INVALID_STRING_NOT_AN_INT_TESTS)
            {
                bool exceptionThrown = false;
                try
                {
                    CustomNumberParser.StringToInt(test);
                }
                catch (ArgumentException)
                {
                    exceptionThrown = true;
                }
                Assert.IsTrue(exceptionThrown);
            }
        }

        [TestMethod]
        public void StringToIntInvalid_ArithmeticOverflow_ArithmeticException()
        {
            foreach (string test in INVALID_ARITHMETIC_OVERFLOW_INT_TESTS)
            {
                bool exceptionThrown = false;
                try
                {
                    CustomNumberParser.StringToInt(test);
                }
                catch (ArithmeticException)
                {
                    exceptionThrown = true;
                }
                Assert.IsTrue(exceptionThrown);
            }
        }

        [TestMethod]
        public void PerformanceOfIntToString_10()
        {
            // This should be run in Release mode, not debug mode.

            Console.WriteLine("Performance testing IntToString");
            float avgTickCount = PerformanceTest(() =>
            {
                foreach ((string _, int num) in NORMAL_INT_TESTS)
                {
                    //num.ToString(); // ~15 ticks
                    CustomNumberParser.IntToString(num);
                }
            });
            Assert.IsTrue(avgTickCount <= 10);
        }

        [TestMethod]
        public void PerformanceOfStringToInt_5()
        {
            // This should be run in Release mode, not debug mode.

            Console.WriteLine("Performance testing StringToInt");
            float avgTickCount = PerformanceTest(() =>
            {
                foreach ((string str, int _) in NORMAL_INT_TESTS)
                {
                    //int.Parse(str); // ~20 ticks
                    CustomNumberParser.StringToInt(str);
                }
            });
            Assert.IsTrue(avgTickCount <= 5);
        }

        #endregion Integers

        #region Floats

        [TestMethod]
        public void FloatToStringNormal()
        {
            foreach ((string expected, float num, float _) in NORMAL_FLOAT_TESTS)
            {
                string actual = CustomNumberParser.FloatToString(num);
                
                // Floating point imprecisions with the last few digits are fine.

                // So just check first 7 characters of the strings are equal,
                int numCharsToCheck = Math.Min(7, Math.Min(expected.Length, actual.Length));
                Assert.AreEqual(expected.Substring(0, numCharsToCheck), actual.Substring(0, numCharsToCheck));
                
                // and if there are no decimal points, the length should be equal
                if (!expected.Contains("."))
                {
                    Assert.AreEqual(expected.Length, actual.Length);
                }
                // but if there is a decimal point, then it's okay if the input/expected string has some extra digits
                else
                {
                    Assert.IsTrue(expected.Length >= actual.Length);
                }
            }
        }

        [TestMethod]
        public void FloatToStringInvalid_NotANumber_ArgumentException()
        {
            bool exceptionThrown = false;
            try
            {
                CustomNumberParser.FloatToString(float.NaN);
            }
            catch (ArgumentException)
            {
                exceptionThrown = true;
            }
            Assert.IsTrue(exceptionThrown);
        }

        [TestMethod]
        public void FloatToStringInvalid_Infinity_ArgumentException()
        {
            bool exceptionThrown = false;
            try
            {
                CustomNumberParser.FloatToString(float.PositiveInfinity);
            }
            catch (ArgumentException)
            {
                exceptionThrown = true;
            }
            Assert.IsTrue(exceptionThrown);
            
            exceptionThrown = false;
            try
            {
                CustomNumberParser.FloatToString(float.NegativeInfinity);
            }
            catch (ArgumentException)
            {
                exceptionThrown = true;
            }
            Assert.IsTrue(exceptionThrown);
        }

        [TestMethod]
        public void StringToFloatNormal()
        {
            foreach ((string str, float num, float epsilon) in NORMAL_FLOAT_TESTS)
            {
                Assert.AreEqual(num, CustomNumberParser.StringToFloat(str), epsilon);
            }
        }

        [TestMethod]
        public void StringToFloatNormal_LeadingAndTrailingZeros()
        {
            Assert.AreEqual(0, CustomNumberParser.StringToFloat("000"), float.Epsilon);
            Assert.AreEqual(0, CustomNumberParser.StringToFloat("00.00"), float.Epsilon);
            Assert.AreEqual(3, CustomNumberParser.StringToFloat("003"), float.Epsilon);
            Assert.AreEqual(3.6f, CustomNumberParser.StringToFloat("003.6"), float.Epsilon);
            Assert.AreEqual(3.6f, CustomNumberParser.StringToFloat("3.600"), float.Epsilon);
            Assert.AreEqual(3.6f, CustomNumberParser.StringToFloat("003.600"), float.Epsilon);
            Assert.AreEqual(3, CustomNumberParser.StringToFloat("00000000000000000000000000000000000000000003"), float.Epsilon);
            Assert.AreEqual(3.6f, CustomNumberParser.StringToFloat("00000000000000000000000000000000000000000003.6"), float.Epsilon);
            Assert.AreEqual(3.6f, CustomNumberParser.StringToFloat("00000000000000000000000000000000000000000003.60000000000000000000000000000000000000000000"), float.Epsilon);
        }

        [TestMethod]
        public void StringToFloatNormal_NegativeZero()
        {
            Assert.AreEqual(0, CustomNumberParser.StringToFloat("-0"), float.Epsilon);
        }

        [TestMethod]
        public void StringToFloatInvalid_NotANumber_ArgumentException()
        {
            foreach (string test in INVALID_STRING_NOT_A_FLOAT_TESTS)
            {
                bool exceptionThrown = false;
                try
                {
                    CustomNumberParser.StringToFloat(test);
                }
                catch (ArgumentException)
                {
                    exceptionThrown = true;
                }
                Assert.IsTrue(exceptionThrown);
            }
        }

        [TestMethod]
        public void StringToFloatInvalid_ArithmeticOverflow_ArithmeticException()
        {
            foreach (string test in INVALID_ARITHMETIC_OVERFLOW_FLOAT_TESTS)
            {
                bool exceptionThrown = false;
                try
                {
                    CustomNumberParser.StringToFloat(test);
                }
                catch (ArithmeticException)
                {
                    exceptionThrown = true;
                }
                Assert.IsTrue(exceptionThrown);
            }
        }

        [TestMethod]
        public void PerformanceOfFloatToString_200()
        {
            // This should be run in Release mode, not debug mode.

            Console.WriteLine("Performance testing FloatToString");
            float avgTickCount = PerformanceTest(() =>
            {
                foreach ((string _, float num, float _) in NORMAL_FLOAT_TESTS)
                {
                    //num.ToString(); // ~ 220 ticks
                    CustomNumberParser.FloatToString(num);
             }
            });
            Assert.IsTrue(avgTickCount <= 200);
        }

        [TestMethod]
        public void PerformanceOfStringToFloat_20()
        {
            // This should be run in Release mode, not debug mode.

            Console.WriteLine("Performance testing StringToFloat");
            float avgTickCount = PerformanceTest(() =>
            {
                foreach ((string str, float _, float _) in NORMAL_FLOAT_TESTS)
                {
                    //float.Parse(str); // ~ 80 ticks
                    CustomNumberParser.StringToFloat(str);
                }
            });
            Assert.IsTrue(avgTickCount <= 20);
        }

        #endregion Floats

        #region Performance

        public float PerformanceTest(Action functionUnderTest)
        {
            // This is copied (with modifications) from https://www.codeproject.com/Articles/61964/Performance-Tests-Precise-Run-Time-Measurements-wi
            // which is copyrighted under a BSD license. https://opensource.org/license/bsd-2-clause/
            // Since this is only being used for unit testing and is not part of the software released in Einstein,
            // I belive I only need to include the BSD license here in the source code and not in Einstein releases.

            // Copyright 2010 Thomas Maierhofer
            // Redistribution and use in source and binary forms, with or without modification, are permitted
            // provided that the following conditions are met:
            // 1. Redistributions of source code must retain the above copyright notice, this list of conditions
            //    and the following disclaimer.
            // 2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions
            //    and the following disclaimer in the documentation and / or other materials provided with the distribution.
            // THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS “AS IS” AND ANY EXPRESS OR IMPLIED WARRANTIES,
            // INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
            // DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
            // SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
            // SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
            // WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE
            // USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

            Stopwatch stopwatch = new Stopwatch();
            long _ = Environment.TickCount;  // Prevents the JIT Compiler from optimizing Fkt calls away

            Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(2); // Uses the second Core or Processor for the Test
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High; // Prevents "Normal" processes from interrupting Threads
            Thread.CurrentThread.Priority = ThreadPriority.Highest; // Prevents "Normal" Threads from interrupting this thread

            stopwatch.Reset();
            stopwatch.Start();
            while (stopwatch.ElapsedMilliseconds < 1200)  // A Warmup of 1000-1500 mS stabilizes the CPU cache and pipeline.
            {
                functionUnderTest.Invoke(); // Warmup
            }
            stopwatch.Stop();

            long elapsedTicksSum = 0;
            long elapsedTicksMin = long.MaxValue;
            long elapsedTicksMax = long.MinValue;
            for (int repeat = 0; repeat < 20; ++repeat)
            {
                stopwatch.Reset();
                stopwatch.Start();
                functionUnderTest.Invoke();
                stopwatch.Stop();

                Console.WriteLine("Ticks: " + stopwatch.ElapsedTicks);
                elapsedTicksSum += stopwatch.ElapsedTicks;
                elapsedTicksMin = Math.Min(elapsedTicksMin, stopwatch.ElapsedTicks);
                elapsedTicksMax = Math.Max(elapsedTicksMax, stopwatch.ElapsedTicks);
            }

            float avgResult = (elapsedTicksSum - elapsedTicksMax - elapsedTicksMin) / 18;
            Console.WriteLine("\nMax ticks: " + elapsedTicksMax);
            Console.WriteLine("Min ticks: " + elapsedTicksMin);
            Console.WriteLine("Avg ticks of all other runs: " + avgResult);

            return avgResult;
        }

        #endregion Performance
    }
}
