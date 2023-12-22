using Einstein.model.json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace EinsteinUnitTests
{
    [TestClass]
    public class CustomNumberParserTests
    {
        #region Integers

        private static readonly (string, int)[] NORMAL_INT_TESTS = new (string, int)[]
        {
            ("0", 0),
            ("1", 1),
            ("-1", -1),
            ("9", 9),
            ("10", 10),
            ("11", 11),
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
            "?",
            "A",
            "4A",
            "A4",
            "0.5",
            ".5",
            "-3.5",
            "-35-",
        };

        private static readonly string[] INVALID_ARITHMETIC_OVERFLOW_INT_TESTS = new string[]
        {
            "2147483648",
            "-2147483649",
        };

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

        #endregion Integers

        #region Floats

        private static readonly (string str, float num, float epsilon)[] NORMAL_FLOAT_TESTS
            = new (string str, float num, float epsilon)[]
        {
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
            ("2147483600", 2147483600, 1000f),
            ("-2147483600", -2147483600, 1000f),
            ("2147.483600", 2147.483600f, 0.001f),
            ("-2147.483600", -2147.483600f, 0.001f),
            ("340282356779000000000000000000000000000", 340282356779000000000000000000000000000f, 10000000000000000000000000000f),
            ("-340282356779000000000000000000000000000", -340282356779000000000000000000000000000f, 10000000000000000000000000000f),
            ("0.3", 0.3f, float.Epsilon),
            ("-3.1", -3.1f, float.Epsilon),
        };

        private static readonly string[] INVALID_STRING_NOT_A_FLOAT_TESTS = new string[]
        {
            "",
            " ",
            "-",
            "?",
            "A",
            "4A",
            "A4",
            "-35-",
        };

        private static readonly string[] INVALID_ARITHMETIC_OVERFLOW_FLOAT_TESTS = new string[]
        {
            "3402823567800000000000000000000000000000",
            "-3402823567800000000000000000000000000000",
        };

        [TestMethod]
        public void FloatToStringNormal()
        {
            foreach ((string str, float num, float _) in NORMAL_FLOAT_TESTS)
            {
                Assert.AreEqual(str, CustomNumberParser.FloatToString(num));
            }
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

        #endregion Floats
    }
}
