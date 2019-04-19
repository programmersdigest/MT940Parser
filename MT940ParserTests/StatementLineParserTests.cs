using Microsoft.VisualStudio.TestTools.UnitTesting;
using programmersdigest.MT940Parser;
using programmersdigest.MT940Parser.Parsing;
using System;
using System.IO;

namespace MT940ParserTests
{
    [TestClass]
    public class StatementLineParserTests
    {
        private StatementLineParser _parser;

        [TestInitialize]
        public void TestInitialize()
        {
            _parser = new StatementLineParser();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _parser = null;
        }

        #region Input checks


        #endregion

        #region Positive data checks

        [TestMethod]
        public void StatementLineParser_CorrectStatementLine_ShouldParse()
        {
            var text = "1802150214CR12,34NMSCNONREF//Bank Reference\r\nSupplementary Details";

            var line = _parser.ReadStatementLine(text);

            Assert.IsNotNull(line);
        }

        [TestMethod]
        public void StatementLineParser_ValueDateIs180215_ShouldParse()
        {
            var text = "1802150214CR12,34NMSCNONREF//Bank Reference\r\nSupplementary Details";

            var line = _parser.ReadStatementLine(text);

            Assert.AreEqual(new DateTime(2018, 02, 15), line.ValueDate);
        }

        [TestMethod]
        public void StatementLineParser_ValueDateIs791231_ShouldParseYearAs2079()
        {
            var text = "7912310214CR12,34NMSCNONREF//Bank Reference\r\nSupplementary Details";

            var line = _parser.ReadStatementLine(text);

            Assert.AreEqual(new DateTime(2079, 12, 31), line.ValueDate);
        }

        [TestMethod]
        public void StatementLineParser_ValueDateIs800101_ShouldParseYearAs1980()
        {
            var text = "8001010214CR12,34NMSCNONREF//Bank Reference\r\nSupplementary Details";

            var line = _parser.ReadStatementLine(text);

            Assert.AreEqual(new DateTime(1980, 01, 01), line.ValueDate);
        }

        [TestMethod]
        public void StatementLineParser_EntryDateIs0214_ShouldParseWithYearFromValueDate()
        {
            var text = "1802150214CR12,34NMSCNONREF//Bank Reference\r\nSupplementary Details";

            var line = _parser.ReadStatementLine(text);

            Assert.AreEqual(new DateTime(2018, 02, 14), line.EntryDate);
        }

        [TestMethod]
        public void StatementLineParser_ValueDateIs180101_EntryDateIs1231_ShouldParseWithLastYearBeforeValueDate()
        {
            var text = "1801011231CR12,34NMSCNONREF//Bank Reference\r\nSupplementary Details";

            var line = _parser.ReadStatementLine(text);

            Assert.AreEqual(new DateTime(2017, 12, 31), line.EntryDate);
        }

        [TestMethod]
        public void StatementLineParser_EntryDateDoesNotExist_ShouldParseAsNull()
        {
            var text = "180101CR12,34NMSCNONREF//Bank Reference\r\nSupplementary Details";

            var line = _parser.ReadStatementLine(text);

            Assert.AreEqual(null, line.EntryDate);
        }

        [TestMethod]
        public void StatementLineParser_FieldDebitCreditIsC_ShouldParseAsCredit()
        {
            var text = "1801011231CR12,34NMSCNONREF//Bank Reference\r\nSupplementary Details";

            var line = _parser.ReadStatementLine(text);

            Assert.AreEqual(DebitCreditMark.Credit, line.Mark);
        }

        [TestMethod]
        public void StatementLineParser_FieldDebitCreditIsD_ShouldParseAsDebit()
        {
            var text = "1801011231DR12,34NMSCNONREF//Bank Reference\r\nSupplementary Details";

            var line = _parser.ReadStatementLine(text);

            Assert.AreEqual(DebitCreditMark.Debit, line.Mark);
        }

        [TestMethod]
        public void StatementLineParser_FieldDebitCreditIsRC_ShouldParseAsReverseCredit()
        {
            var text = "1801011231RCR12,34NMSCNONREF//Bank Reference\r\nSupplementary Details";

            var line = _parser.ReadStatementLine(text);

            Assert.AreEqual(DebitCreditMark.ReverseCredit, line.Mark);
        }

        [TestMethod]
        public void StatementLineParser_FieldDebitCreditIsRD_ShouldParseAsReverseDebit()
        {
            var text = "1801011231RDR12,34NMSCNONREF//Bank Reference\r\nSupplementary Details";

            var line = _parser.ReadStatementLine(text);

            Assert.AreEqual(DebitCreditMark.ReverseDebit, line.Mark);
        }

        [TestMethod]
        public void StatementLineParser_FundsCodeIsR_ShouldParseAsR()
        {
            var text = "1801011231RDR12,34NMSCNONREF//Bank Reference\r\nSupplementary Details";

            var line = _parser.ReadStatementLine(text);

            Assert.AreEqual('R', line.FundsCode);
        }

        [TestMethod]
        public void StatementLineParser_FundsCodeIsF_ShouldParseAsF()
        {
            var text = "1801011231RDF12,34NMSCNONREF//Bank Reference\r\nSupplementary Details";

            var line = _parser.ReadStatementLine(text);

            Assert.AreEqual('F', line.FundsCode);
        }

        [TestMethod]
        public void StatementLineParser_FundsCodeDoesNotExist_ShouldParseAsNull()
        {
            var text = "1801011231RD12,34NMSCNONREF//Bank Reference\r\nSupplementary Details";

            var line = _parser.ReadStatementLine(text);

            Assert.AreEqual(null, line.FundsCode);
        }

        [TestMethod]
        public void StatementLineParser_AmountIs0_ShouldParseAs0()
        {
            var text = "1801011231CR0NMSCNONREF//Bank Reference\r\nSupplementary Details";

            var line = _parser.ReadStatementLine(text);

            Assert.AreEqual(0m, line.Amount);
        }

        [TestMethod]
        public void StatementLineParser_AmountIsInteger_ShouldParse()
        {
            var text = "1801011231CR1234NMSCNONREF//Bank Reference\r\nSupplementary Details";

            var line = _parser.ReadStatementLine(text);

            Assert.AreEqual(1234m, line.Amount);
        }

        [TestMethod]
        public void StatementLineParser_AmountIsDecimal_ShouldParse()
        {
            var text = "1801011231CR12,34NMSCNONREF//Bank Reference\r\nSupplementary Details";

            var line = _parser.ReadStatementLine(text);

            Assert.AreEqual(12.34m, line.Amount);
        }

        [TestMethod]
        public void StatementLineParser_TransactionType_F_ShouldParse()
        {
            var text = "1801011231CR12,34FABCNONREF//Bank Reference\r\nSupplementary Details";

            var line = _parser.ReadStatementLine(text);

            Assert.AreEqual("ABC", line.TransactionTypeIdCode);
        }

        [TestMethod]
        public void StatementLineParser_TransactionType_N_ShouldParse()
        {
            var text = "1801011231CR12,34NMSCNONREF//Bank Reference\r\nSupplementary Details";

            var line = _parser.ReadStatementLine(text);

            Assert.AreEqual("MSC", line.TransactionTypeIdCode);
        }

        [TestMethod]
        public void StatementLineParser_TransactionType_S_ShouldParse()
        {
            var text = "1801011231CR12,34SXYZNONREF//Bank Reference\r\nSupplementary Details";

            var line = _parser.ReadStatementLine(text);

            Assert.AreEqual("XYZ", line.TransactionTypeIdCode);
        }

        [TestMethod]
        public void StatementLineParser_CustomerRefIsValid_ShouldParse()
        {
            var text = "1801011231CR12,34NABCNONREF//Bank Reference\r\nSupplementary Details";

            var line = _parser.ReadStatementLine(text);

            Assert.AreEqual("NONREF", line.CustomerReference);
        }

        [TestMethod]
        public void StatementLineParser_CustomerRefFullLength_ShouldParse()
        {
            var text = "1801011231CR12,34NABC1234567890123456//Bank Reference\r\nSupplementary Details";

            var line = _parser.ReadStatementLine(text);

            Assert.AreEqual("1234567890123456", line.CustomerReference);
        }

        [TestMethod]
        public void StatementLineParser_CustomerRefFollowedByBankRef_ShouldEndOnDoubleslash()
        {
            var text = "1801011231CR12,34NABCCust Ref//Bank Ref\r\nSupplementary Details";

            var line = _parser.ReadStatementLine(text);

            Assert.AreEqual("Cust Ref", line.CustomerReference);
        }

        [TestMethod]
        public void StatementLineParser_CustomerRefFollowedBySupplementaryDetails_ShouldEndOnNewline()
        {
            var text = "1801011231CR12,34NABCCust Ref\r\nSupplementary Details";

            var line = _parser.ReadStatementLine(text);

            Assert.AreEqual("Cust Ref", line.CustomerReference);
        }

        [TestMethod]
        public void StatementLineParser_BankRefIsValid_ShouldParse()
        {
            var text = "1801011231CR12,34NABCNONREF//Bank Reference\r\nSupplementary Details";

            var line = _parser.ReadStatementLine(text);

            Assert.AreEqual("Bank Reference", line.BankReference);
        }

        [TestMethod]
        public void StatementLineParser_BankRefFullLength_ShouldParse()
        {
            var text = "1801011231CR12,34NABCNONREF//1234567890123456\r\nSupplementary Details";

            var line = _parser.ReadStatementLine(text);

            Assert.AreEqual("1234567890123456", line.BankReference);
        }

        [TestMethod]
        public void StatementLineParser_BankRefDoesNotExist_ShouldParseAsNull()
        {
            var text = "1801011231CR12,34NABCNONREF\r\nSupplementary Details";

            var line = _parser.ReadStatementLine(text);

            Assert.AreEqual(null, line.BankReference);
        }

        [TestMethod]
        public void StatementLineParser_SupplementaryDetailsIsValid_ShouldParse()
        {
            var text = "1801011231CR12,34NABCNONREF//Bank Reference\r\nSupplementary Details";

            var line = _parser.ReadStatementLine(text);

            Assert.AreEqual("Supplementary Details", line.SupplementaryDetails);
        }

        [TestMethod]
        public void StatementLineParser_SupplementaryDetailsFullLength_ShouldParse()
        {
            var text = "1801011231CR12,34NABCNONREF\r\n1234567890123456789012345678901234";

            var line = _parser.ReadStatementLine(text);

            Assert.AreEqual("1234567890123456789012345678901234", line.SupplementaryDetails);
        }

        [TestMethod]
        public void StatementLineParser_SupplementaryDetailsDoesNotExist_ShouldParseAsNull()
        {
            var text = "1801011231CR12,34NABCNONREF//Bank Reference";

            var line = _parser.ReadStatementLine(text);

            Assert.AreEqual(null, line.SupplementaryDetails);
        }

        #endregion

        #region Mandatory fields

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void StatementLineParser_ValueDateDoesNotExist_ShouldThrowInvalidDataException()
        {
            var text = "";

            _parser.ReadStatementLine(text);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void StatementLineParser_OnlyValueDate_ShouldThrowInvalidDataException()
        {
            var text = "180101";

            _parser.ReadStatementLine(text);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void StatementLineParser_ValueDateAndDebitCreditMark_ShouldThrowInvalidDataException()
        {
            var text = "180101C";

            _parser.ReadStatementLine(text);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void StatementLineParser_ValueDateMarkAndAmount_ShouldThrowInvalidDataException()
        {
            var text = "180101C12,34";

            _parser.ReadStatementLine(text);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void StatementLineParser_ValueDateMarkAmountAndTransactionTypeIdCode_ShouldThrowInvalidDataException()
        {
            var text = "180101C12,34NMSC";

            _parser.ReadStatementLine(text);
        }

        #endregion

        #region Negative value checks

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void StatementLineParser_ValueDateTooShort_ShouldThrowInvalidDataException()
        {
            var text = "1801";

            _parser.ReadStatementLine(text);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void StatementLineParser_ValueDateInvalid_ShouldThrowArgumentOutOfRangeException()
        {
            var text = "9999991231CR12,34NMSCNONREF//Bank Reference\r\nSupplementary Details";

            _parser.ReadStatementLine(text);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void StatementLineParser_EntryDateTooShort_ShouldThrowInvalidDataException()
        {
            var text = "18010112CR12,34NMSCNONREF//Bank Reference\r\nSupplementary Details";

            _parser.ReadStatementLine(text);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void StatementLineParser_EntryDateInvalid_ShouldThrowArgumentOutOfRangeException()
        {
            var text = "1801019999CR12,34NMSCNONREF//Bank Reference\r\nSupplementary Details";

            _parser.ReadStatementLine(text);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void StatementLineParser_DebitCreditMarkDoesNotExist_ShouldThrowFormatException()
        {
            var text = "1801011231N12,34NMSCNONREF//Bank Reference\r\nSupplementary Details";

            _parser.ReadStatementLine(text);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void StatementLineParser_DebitCreditMarkIsInvalidValue_ShouldThrowFormatException()
        {
            var text = "1801011231AR12,34NMSCNONREF//Bank Reference\r\nSupplementary Details";

            _parser.ReadStatementLine(text);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void StatementLineParser_AmountDoesNotExist_ShouldThrowInvalidDataException()
        {
            var text = "1801011231CRNMSCNONREF//Bank Reference\r\nSupplementary Details";

            _parser.ReadStatementLine(text);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void StatementLineParser_AmountContainsMultipleCommas_ShouldThrowFormatException()
        {
            var text = "1801011231CR12,34,56NMSCNONREF//Bank Reference\r\nSupplementary Details";

            _parser.ReadStatementLine(text);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void StatementLineParser_CustomerRefDoesNotExist_ShouldThrowInvalidDataException()
        {
            var text = "1801011231CR12,34ABCD";

            _parser.ReadStatementLine(text);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void StatementLineParser_CustomerRefTooLong_ShouldThrowInvalidDataException()
        {
            var text = "1801011231CR12,34ABCD12345678901234567";

            _parser.ReadStatementLine(text);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void StatementLineParser_BankRefTooLong_ShouldThrowInvalidDataException()
        {
            var text = "1801011231CR12,34ABCDCustomer Ref//12345678901234567";

            _parser.ReadStatementLine(text);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void StatementLineParser_SupplementaryDetailsTooLong_ShouldThrowInvalidDataException()
        {
            var text = "1801011231CR12,34ABCDCustomer Ref//Bank Ref\r\n12345678901234567890123456789012345";

            _parser.ReadStatementLine(text);
        }

        [DataTestMethod]
        [DataRow('A')]
        [DataRow('B')]
        [DataRow('C')]
        [DataRow('D')]
        [DataRow('E')]
        [DataRow('G')]
        [DataRow('H')]
        [DataRow('I')]
        [DataRow('J')]
        [DataRow('K')]
        [DataRow('L')]
        [DataRow('M')]
        [DataRow('O')]
        [DataRow('P')]
        [DataRow('Q')]
        [DataRow('R')]
        [DataRow('T')]
        [DataRow('U')]
        [DataRow('V')]
        [DataRow('W')]
        [DataRow('X')]
        [DataRow('Y')]
        [DataRow('Z')]
        [ExpectedException(typeof(InvalidDataException))]
        public void StatementLineParser_TransactionTypeInvalid_ShouldThrowInvalidDataException(char transactionType)
        {
            var text = "1801011231CR12,34" + transactionType + "MSCNONREF//Bank Reference\r\nSupplementary Details";

            _parser.ReadStatementLine(text);
        }

        #endregion
    }
}
