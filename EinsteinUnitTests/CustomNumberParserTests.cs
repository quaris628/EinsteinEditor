using Einstein.model.json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace EinsteinUnitTests
{
    [TestClass]
    public class CustomNumberParserTests
    {
        private static readonly (string str, int i)[] NORMAL_TESTS = new (string str, int i)[]
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

        private static readonly string[] INVALID_STRING_NOT_A_NUMBER_TESTS = new string[]
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

        private static readonly string[] INVALID_ARITHMETIC_OVERFLOW_TESTS = new string[]
        {
            "2147483648",
            "-2147483649",
        };

        [TestMethod]
        public void IntToStringNormal()
        {
            foreach ((string str, int i) test in NORMAL_TESTS)
            {
                Assert.AreEqual(test.str, CustomNumberParser.IntToString(test.i));
            }
        }

        [TestMethod]
        public void StringToIntNormal()
        {
            foreach ((string str, int i) test in NORMAL_TESTS)
            {
                Assert.AreEqual(test.i, CustomNumberParser.StringToInt(test.str));
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
            foreach (string test in INVALID_STRING_NOT_A_NUMBER_TESTS)
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
            foreach (string test in INVALID_ARITHMETIC_OVERFLOW_TESTS)
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

    }
}
