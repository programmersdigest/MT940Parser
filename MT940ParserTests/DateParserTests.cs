using Microsoft.VisualStudio.TestTools.UnitTesting;
using programmersdigest.MT940Parser.Parsing;
using System;

namespace MT940ParserTests
{
    [TestClass]
    public class DateParserTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DateParser_Parse_DateIsNull_ShouldThrowArgumentNullException()
        {
            DateParser.Parse(null);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void DateParser_Parse_DateIsEmpty_ShouldThrowFormatException()
        {
            DateParser.Parse("");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void DateParser_Parse_DateIsTooShort_ShouldThrowFormatException()
        {
            DateParser.Parse("1805");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void DateParser_Parse_DateIsTooLong_ShouldThrowFormatException()
        {
            DateParser.Parse("18050317");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void DateParser_Parse_DateContainsAlphaCharacters_ShouldThrowFormatException()
        {
            DateParser.Parse("18ab02");
        }

        [TestMethod]
        public void DateParser_Parse_DateIs180220_ShouldParseCorrectly()
        {
            var result = DateParser.Parse("180220");

            Assert.AreEqual(new DateTime(2018, 02, 20), result);
        }

        [TestMethod]
        public void DateParser_Parse_DateIs791231_ShouldParseAs2079()
        {
            var result = DateParser.Parse("791231");

            Assert.AreEqual(new DateTime(2079, 12, 31), result);
        }

        [TestMethod]
        public void DateParser_Parse_DateIs800101_ShouldParseAs1980()
        {
            var result = DateParser.Parse("800101");

            Assert.AreEqual(new DateTime(1980, 01, 01), result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DateParser_Parse_DateIsImpossible_eg189901_ShouldThrowArgumentOutOfRangeException()
        {
            DateParser.Parse("189901");
        }
    }
}
