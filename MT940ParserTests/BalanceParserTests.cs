using Microsoft.VisualStudio.TestTools.UnitTesting;
using programmersdigest.MT940Parser;
using programmersdigest.MT940Parser.Parsing;
using System;
using System.IO;

namespace MT940ParserTests
{
    [TestClass]
    public class BalanceParserTests
    {
        private BalanceParser _parser;

        [TestInitialize]
        public void TestInitialize()
        {
            _parser = new BalanceParser();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _parser = null;
        }

        #region Optional and mandatory fields

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BalanceParser_BufferIsNull_ShouldThrowArgumentNullException()
        {
            _parser.ReadBalance(null, BalanceType.None);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void BalanceParser_StringIsEmpty_ShouldThrowInvalidDataException()
        {
            _parser.ReadBalance("", BalanceType.None);
        }

        #endregion

        #region Positive data tests

        [TestMethod]
        public void BalanceParser_CompleteBalance_ShouldParse()
        {
            var text = "C180215EUR12,34";
            var balance = _parser.ReadBalance(text, BalanceType.None);

            Assert.IsNotNull(balance);
        }

        [TestMethod]
        public void BalanceParser_BalanceTypeIsNone_ShouldParseAsNone()
        {
            var text = "C180215EUR12,34";
            var balance = _parser.ReadBalance(text, BalanceType.None);

            Assert.AreEqual(BalanceType.None, balance.Type);
        }

        [TestMethod]
        public void BalanceParser_BalanceTypeIsOpening_ShouldParseAsOpening()
        {
            var text = "C180215EUR12,34";
            var balance = _parser.ReadBalance(text, BalanceType.Opening);

            Assert.AreEqual(BalanceType.Opening, balance.Type);
        }

        [TestMethod]
        public void BalanceParser_BalanceTypeIsClosing_ShouldParseAsClosing()
        {
            var text = "C180215EUR12,34";
            var balance = _parser.ReadBalance(text, BalanceType.Closing);

            Assert.AreEqual(BalanceType.Closing, balance.Type);
        }

        [TestMethod]
        public void BalanceParser_BalanceTypeIsIntermediate_ShouldParseAsIntermediate()
        {
            var text = "C180215EUR12,34";
            var balance = _parser.ReadBalance(text, BalanceType.Intermediate);

            Assert.AreEqual(BalanceType.Intermediate, balance.Type);
        }

        [TestMethod]
        public void BalanceParser_FieldDebitCreditIsC_ShouldParseAsCredit()
        {
            var text = "C180215EUR12,34";
            var balance = _parser.ReadBalance(text, BalanceType.None);

            Assert.AreEqual(DebitCreditMark.Credit, balance.Mark);
        }

        [TestMethod]
        public void BalanceParser_FieldDebitCreditIsD_ShouldParseAsDebit()
        {
            var text = "D180215EUR12,34";
            var balance = _parser.ReadBalance(text, BalanceType.None);

            Assert.AreEqual(DebitCreditMark.Debit, balance.Mark);
        }

        [TestMethod]
        public void BalanceParser_FieldDateIs180215_ShouldParseDate()
        {
            var text = "D180215EUR12,34";
            var balance = _parser.ReadBalance(text, BalanceType.None);

            Assert.AreEqual(new DateTime(2018, 02, 15), balance.Date);
        }

        [TestMethod]
        public void BalanceParser_FieldDateIs791231_ShouldParseYearAs1979()
        {
            var text = "D791231EUR12,34";
            var balance = _parser.ReadBalance(text, BalanceType.None);

            Assert.AreEqual(new DateTime(2079, 12, 31), balance.Date);
        }

        [TestMethod]
        public void BalanceParser_FieldDateIs800101_ShouldParseYearAs1980()
        {
            var text = "D800101EUR12,34";
            var balance = _parser.ReadBalance(text, BalanceType.None);

            Assert.AreEqual(new DateTime(1980, 01, 01), balance.Date);
        }

        [TestMethod]
        public void BalanceParser_FieldCurrencyIsEUR_ShouldParseAsEUR()
        {
            var text = "D991231EUR12,34";
            var balance = _parser.ReadBalance(text, BalanceType.None);

            Assert.AreEqual("EUR", balance.Currency);
        }

        [TestMethod]
        public void BalanceParser_FieldCurrencyIsUSD_ShouldParseAsUSD()
        {
            var text = "D991231USD12,34";
            var balance = _parser.ReadBalance(text, BalanceType.None);

            Assert.AreEqual("USD", balance.Currency);
        }

        [TestMethod]
        public void BalanceParser_FieldCurrencyIsCHF_ShouldParseAsCHF()
        {
            var text = "D991231CHF12,34";
            var balance = _parser.ReadBalance(text, BalanceType.None);

            Assert.AreEqual("CHF", balance.Currency);
        }

        [TestMethod]
        public void BalanceParser_FieldAmountIs0_ShouldParseAs0()
        {
            var text = "D991231CHF0";
            var balance = _parser.ReadBalance(text, BalanceType.None);

            Assert.AreEqual(0m, balance.Amount);
        }

        [TestMethod]
        public void BalanceParser_FieldAmountIsInteger_ShouldParse()
        {
            var text = "D991231CHF12345";
            var balance = _parser.ReadBalance(text, BalanceType.None);

            Assert.AreEqual(12345m, balance.Amount);
        }

        [TestMethod]
        public void BalanceParser_FieldAmountIsDecimal_ShouldParse()
        {
            var text = "D991231CHF123,456";
            var balance = _parser.ReadBalance(text, BalanceType.None);

            Assert.AreEqual(123.456m, balance.Amount);
        }

        #endregion

        #region Negative data tests

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void BalanceParser_FieldDebitCreditIsInvalid_ShouldThrowFormatException()
        {
            var text = "A180215EUR12,34";
            _parser.ReadBalance(text, BalanceType.None);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void BalanceParser_FieldDateIs000000_ShouldThrowArgumentOutOfRangeException()
        {
            var text = "D000000EUR12,34";
            _parser.ReadBalance(text, BalanceType.None);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void BalanceParser_FieldDateIs999999_ShouldThrowArgumentOutOfRangeException()
        {
            var text = "D999999EUR12,34";
            _parser.ReadBalance(text, BalanceType.None);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void BalanceParser_FieldDateIsNonNumeric_ShouldThrowInvalidDataException()
        {
            var text = "DabcdefEUR12,34";
            _parser.ReadBalance(text, BalanceType.None);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void BalanceParser_FieldCurrencyHasNumbers_ShouldThrowInvalidDataException()
        {
            var text = "D991231E8R12,34";
            _parser.ReadBalance(text, BalanceType.None);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void BalanceParser_FieldCurrencyHasLowercase_ShouldThrowInvalidDataException()
        {
            var text = "D991231eur12,34";
            _parser.ReadBalance(text, BalanceType.None);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void BalanceParser_FieldCurrencyHasWhitespace_ShouldThrowInvalidDataException()
        {
            var text = "D991231E R12,34";
            _parser.ReadBalance(text, BalanceType.None);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void BalanceParser_FieldAmountIsEmpty_ShouldThrowInvalidDataException()
        {
            var text = "D991231CHF";
            _parser.ReadBalance(text, BalanceType.None);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void BalanceParser_FieldAmountIsNonNumeric_ShouldThrowFormatException()
        {
            var text = "D991231CHF12,AB45";
            _parser.ReadBalance(text, BalanceType.None);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void BalanceParser_FieldAmountIsComma_ShouldThrowFormatException()
        {
            var text = "D991231CHF,";
            _parser.ReadBalance(text, BalanceType.None);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void BalanceParser_FieldAmountHasDot_ShouldThrowFormatException()
        {
            var text = "D991231CHF1.23";
            _parser.ReadBalance(text, BalanceType.None);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void BalanceParser_FieldAmountHasMultipleCommas_ShouldThrowFormatException()
        {
            var text = "D991231CHF12,34,56";
            _parser.ReadBalance(text, BalanceType.None);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void BalanceParser_Empty_ShouldThrowInvalidDataException()
        {
            var text = "";
            _parser.ReadBalance(text, BalanceType.None);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void BalanceParser_OnlyDebitCreaditMark_ShouldThrowInvalidDataException()
        {
            var text = "C";
            _parser.ReadBalance(text, BalanceType.None);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void BalanceParser_OnlyMarkAndDate_ShouldThrowInvalidDataException()
        {
            var text = "C180215";
            _parser.ReadBalance(text, BalanceType.None);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void BalanceParser_DateTooShort_ShouldThrowInvalidDataException()
        {
            var text = "C1802EUR12,34";
            _parser.ReadBalance(text, BalanceType.None);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void BalanceParser_CurrencyTooShort_ShouldThrowInvalidDataException()
        {
            var text = "C180215E12,34";
            _parser.ReadBalance(text, BalanceType.None);
        }

        #endregion
    }
}
