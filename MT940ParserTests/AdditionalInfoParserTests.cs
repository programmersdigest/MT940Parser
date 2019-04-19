using Microsoft.VisualStudio.TestTools.UnitTesting;
using programmersdigest.MT940Parser.Parsing;
using System;
using System.IO;

namespace MT940ParserTests
{
    [TestClass]
    public class AdditionalInfoParserTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AdditionalInfoParser_ParseInformation_ValueIsNull_ShouldThrowArgumentNullException()
        {
            var parser = new AdditionalInfoParser();
            parser.ParseInformation(null);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_ValueIsEmpty_ShouldParseAsUnstructuredData()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("");

            Assert.IsTrue(information.IsUnstructuredData);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_ValueIsRandomData_ShouldParseAsUnstructuredData()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("This is a random text.");

            Assert.IsTrue(information.IsUnstructuredData);
            Assert.AreEqual("This is a random text.", information.UnstructuredData);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_ValueStartsWithTransactionCode999_ShouldParseAsUnstructuredData()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("999This is unstructured data");

            Assert.IsTrue(information.IsUnstructuredData);
            Assert.AreEqual("This is unstructured data", information.UnstructuredData);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_LongSeparatorFormat_ShouldParseAsUnstructuredData()
        {
            // Format uses separators in the for /ABCD/ where ABCD is a specific field code.
            // This format is not currently supported.

            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("/ABCD/Test data");

            Assert.IsTrue(information.IsUnstructuredData);
            Assert.AreEqual("/ABCD/Test data", information.UnstructuredData);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void AdditionalInfoParser_ParseInformation_OnlyTransactionCode_ShouldThrowInvalidDataException()
        {
            var parser = new AdditionalInfoParser();
            parser.ParseInformation("123");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void AdditionalInfoParser_ParseInformation_OnlySeparator_ShouldThrowInvalidDataException()
        {
            var parser = new AdditionalInfoParser();
            parser.ParseInformation("123?");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void AdditionalInfoParser_ParseInformation_FieldCodeTooShort_ShouldThrowInvalidDataException()
        {
            var parser = new AdditionalInfoParser();
            parser.ParseInformation("123?0");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void AdditionalInfoParser_ParseInformation_UnknownFieldCode_ShouldThrowInvalidDataException()
        {
            var parser = new AdditionalInfoParser();
            parser.ParseInformation("123?01");
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_ValidData_ShouldParseTransactionCode()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?00Posting text");

            Assert.AreEqual(123, information.TransactionCode);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_ValidData_ShouldNotBeUnstructuredData()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?00Posting text");

            Assert.IsFalse(information.IsUnstructuredData);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode00_ShouldParsePostingText()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?00Posting text");

            Assert.AreEqual("Posting text", information.PostingText);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode00_ContainsSeparator_ShouldParseCompletePostingText()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?00Posting?text");

            Assert.AreEqual("Posting?text", information.PostingText);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode10_ShouldParseJournalNumber()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?10Journal number");

            Assert.AreEqual("Journal number", information.JournalNumber);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode10_ContainsSeparator_ShouldParseCompleteJournalNumber()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?10Journal?number");

            Assert.AreEqual("Journal?number", information.JournalNumber);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode30_ShouldParseBankCodeOfPayer()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?30Bank code of payer");

            Assert.AreEqual("Bank code of payer", information.BankCodeOfPayer);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode30_ContainsSeparator_ShouldParseCompleteBankCodeOfPayer()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?30Bank?code of?payer");

            Assert.AreEqual("Bank?code of?payer", information.BankCodeOfPayer);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode31_ShouldParseAccountNumberOfPayer()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?31Account number of payer");

            Assert.AreEqual("Account number of payer", information.AccountNumberOfPayer);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode31_ContainsSeparator_ShouldParseCompleteAccountNumberOfPayer()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?31Account?number?of payer");

            Assert.AreEqual("Account?number?of payer", information.AccountNumberOfPayer);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode32_ShouldParseNameOfPayer()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?32Name of payer");

            Assert.AreEqual("Name of payer", information.NameOfPayer);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode33_ShouldParseNameOfPayer()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?33Name of payer");

            Assert.AreEqual("Name of payer", information.NameOfPayer);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode32And33_ShouldParseNameOfPayer()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?32Name of ?33payer");

            Assert.AreEqual("Name of payer", information.NameOfPayer);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode32Ans33_ContainsSeparator_ShouldParseCompleteNameOfPayer()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?32Name?of?33 the?payer");

            Assert.AreEqual("Name?of the?payer", information.NameOfPayer);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode34_ShouldParseTextKeyAddition()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?34345");

            Assert.AreEqual(345, information.TextKeyAddition);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void AdditionalInfoParser_ParseInformation_FieldCode34HasAlphaCharacters_ShouldThrowFormatException()
        {
            var parser = new AdditionalInfoParser();
            parser.ParseInformation("123?34abc");
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20IsEmpty_ShouldParseAsUnstructuredRemittanceInformation()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20");

            Assert.AreEqual("", information.UnstructuredRemittanceInformation);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20UnknownIdentifier_ShouldParseUnstructuredRemittanceInformation()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20ABCD+");

            Assert.AreEqual("ABCD+", information.UnstructuredRemittanceInformation);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20UnknownIdentifier_ContainsSeparator_ShouldParseCompleteUnstructuredRemittanceInformation()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20This is?some rather unstructured?remittance information");

            Assert.AreEqual("This is?some rather unstructured?remittance information", information.UnstructuredRemittanceInformation);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20UnknownIdentifier_MultiPart_ShouldParseUnstructuredRemittanceInformation()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20First line?21Second line");

            Assert.AreEqual("First line\r\nSecond line", information.UnstructuredRemittanceInformation);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20EREF_ShouldParseEndToEndReference()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20EREF+End to end reference");

            Assert.AreEqual("End to end reference", information.EndToEndReference);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20EREF_ContainsSeparator_ShouldParseCompleteEndToEndReference()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20EREF+End to?end reference");

            Assert.AreEqual("End to?end reference", information.EndToEndReference);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20EREF_MultiPart_ShouldParseEndToEndReference()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20EREF+End to end?21 reference");

            Assert.AreEqual("End to end reference", information.EndToEndReference);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20EREF_MultiPart_ContainsSeparator_ShouldParseCompleteEndToEndReference()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20EREF+End to?end?21 reference");

            Assert.AreEqual("End to?end reference", information.EndToEndReference);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20KREF_ShouldParseCustomerReference()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20KREF+Customer reference");

            Assert.AreEqual("Customer reference", information.CustomerReference);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20KREF_ContainsSeparator_ShouldParseCompleteCustomerReference()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20KREF+Customer?reference");

            Assert.AreEqual("Customer?reference", information.CustomerReference);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20KREF_MultiPart_ShouldParseCustomerReference()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20KREF+Customer?21 reference");

            Assert.AreEqual("Customer reference", information.CustomerReference);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20KREF_MultiPart_ContainsSeparator_ShouldParseCompleteCustomerReference()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20KREF+The?Customer?21 reference");

            Assert.AreEqual("The?Customer reference", information.CustomerReference);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20MREF_ShouldParseMandateReference()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20MREF+Mandate reference");

            Assert.AreEqual("Mandate reference", information.MandateReference);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20MREF_ContainsSeparator_ShouldParseCompleteMandateReference()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20MREF+Mandate?reference");

            Assert.AreEqual("Mandate?reference", information.MandateReference);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20MREF_MultiPart_ShouldParseMandateReference()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20MREF+Mandate?21 reference");

            Assert.AreEqual("Mandate reference", information.MandateReference);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20MREF_MultiPart_ContainsSeparator_ShouldParseCompleteMandateReference()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20MREF+Mandate?21 ref?erence");

            Assert.AreEqual("Mandate ref?erence", information.MandateReference);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20CRED_ShouldParseCreditorReference()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20CRED+Creditor reference");

            Assert.AreEqual("Creditor reference", information.CreditorReference);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20CRED_ContainsSeparator_ShouldParseCompleteCreditorReference()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20CRED+Creditor?reference");

            Assert.AreEqual("Creditor?reference", information.CreditorReference);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20CRED_MultiPart_ShouldParseCreditorReference()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20CRED+Creditor?21 reference");

            Assert.AreEqual("Creditor reference", information.CreditorReference);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20CRED_MultiPart_ContainsSeparator_ShouldParseCompleteCreditorReference()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20CRED+Creditor?21 ref?erence");

            Assert.AreEqual("Creditor ref?erence", information.CreditorReference);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20DEBT_ShouldParseOriginatorsIdentificationCode()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20DEBT+Originators identification code");

            Assert.AreEqual("Originators identification code", information.OriginatorsIdentificationCode);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20DEBT_ContainsSeparator_ShouldParseCompleteOriginatorsIdentificationCode()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20DEBT+Originators?identification?code");

            Assert.AreEqual("Originators?identification?code", information.OriginatorsIdentificationCode);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20DEBT_MultiPart_ShouldParseOriginatorsIdentificationCode()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20DEBT+Originators?21 identification code");

            Assert.AreEqual("Originators identification code", information.OriginatorsIdentificationCode);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20DEBT_MultiPart_ContainsSeparator_ShouldParseCompleteOriginatorsIdentificationCode()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20DEBT+Originators?21?identification code");

            Assert.AreEqual("Originators?identification code", information.OriginatorsIdentificationCode);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20COAM_ShouldParseCompensationAmount()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20COAM+Compensation amount");

            Assert.AreEqual("Compensation amount", information.CompensationAmount);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20COAM_ContainsSeparator_ShouldParseCompleteCompensationAmount()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20COAM+Compensation?amount");

            Assert.AreEqual("Compensation?amount", information.CompensationAmount);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20COAM_MultiPart_ShouldParseCompensationAmount()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20COAM+Compensation?21 amount");

            Assert.AreEqual("Compensation amount", information.CompensationAmount);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20COAM_MultiPart_ContainsSeparator_ShouldParseCompleteCompensationAmount()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20COAM+Compensation?21?amount");

            Assert.AreEqual("Compensation?amount", information.CompensationAmount);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20OAMT_ShouldParseOriginalAmount()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20OAMT+Original amount");

            Assert.AreEqual("Original amount", information.OriginalAmount);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20OAMT_ContainsSeparator_ShouldParseCompleteOriginalAmount()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20OAMT+Original?amount");

            Assert.AreEqual("Original?amount", information.OriginalAmount);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20OAMT_MultiPart_ShouldParseOriginalAmount()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20OAMT+Original?21 amount");

            Assert.AreEqual("Original amount", information.OriginalAmount);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20OAMT_MultiPart_ContainsSeparator_ShouldParseCompleteOriginalAmount()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20OAMT+Original?21?amount");

            Assert.AreEqual("Original?amount", information.OriginalAmount);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20SVWZ_ShouldParseSepaRemittanceInformation()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20SVWZ+Sepa remittance information");

            Assert.AreEqual("Sepa remittance information", information.SepaRemittanceInformation);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20SVWZ_ContainsSeparator_ShouldParseCompleteSepaRemittanceInformation()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20SVWZ+Sepa?remittance?information");

            Assert.AreEqual("Sepa?remittance?information", information.SepaRemittanceInformation);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20SVWZ_MultiPart_ShouldParseSepaRemittanceInformation()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20SVWZ+Sepa remittance?21information");

            Assert.AreEqual("Sepa remittance\r\ninformation", information.SepaRemittanceInformation);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20SVWZ_MultiPart_ContainsSeparator_ShouldParseCompleteSepaRemittanceInformation()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20SVWZ+Sepa?remittance?21information");

            Assert.AreEqual("Sepa?remittance\r\ninformation", information.SepaRemittanceInformation);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20SVWZ_MultiPart_ShouldAddNewlineForEachNewField()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20SVWZ+Sepa?21remittance?22information");

            Assert.AreEqual("Sepa\r\nremittance\r\ninformation", information.SepaRemittanceInformation);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20ABWA_ShouldParsePayersReferenceParty()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20ABWA+Payers reference party");

            Assert.AreEqual("Payers reference party", information.PayersReferenceParty);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20ABWA_ContainsSeparator_ShouldParseCompletePayersReferenceParty()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20ABWA+Payers reference?party");

            Assert.AreEqual("Payers reference?party", information.PayersReferenceParty);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20ABWA_MultiPart_ShouldParsePayersReferenceParty()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20ABWA+Payers reference?21 party");

            Assert.AreEqual("Payers reference party", information.PayersReferenceParty);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20ABWA_MultiPart_ContainsSeparator_ShouldParseCompletePayersReferenceParty()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20ABWA+Payers?reference?21 party");

            Assert.AreEqual("Payers?reference party", information.PayersReferenceParty);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20ABWE_ShouldParseCreditorsReferenceParty()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20ABWE+Creditors reference party");

            Assert.AreEqual("Creditors reference party", information.CreditorsReferenceParty);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20ABWE_ContainsSeparator_ShouldParseCompleteCreditorsReferenceParty()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20ABWE+Creditors?reference party");

            Assert.AreEqual("Creditors?reference party", information.CreditorsReferenceParty);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20ABWE_MultiPart_ShouldParseCreditorsReferenceParty()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20ABWE+Creditors reference?21 party");

            Assert.AreEqual("Creditors reference party", information.CreditorsReferenceParty);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_FieldCode20ABWE_MultiPart_ContainsSeparator_ShouldParseCompleteCreditorsReferenceParty()
        {
            var parser = new AdditionalInfoParser();
            var information = parser.ParseInformation("123?20ABWE+Creditors?reference?21?party");

            Assert.AreEqual("Creditors?reference?party", information.CreditorsReferenceParty);
        }

        [TestMethod]
        public void AdditionalInfoParser_ParseInformation_UnstructuredRemittanceInformationAfterStructuredRemittanceInformation_ShouldNotRetainLastRemittanceIdentifier()
        {
            var parser = new AdditionalInfoParser();
            parser.ParseInformation("123?20ABWE+Creditors reference?21 party");

            var information = parser.ParseInformation("123?20Unstructured Data");
            Assert.AreNotEqual("Unstructured Data", information.CreditorsReferenceParty);
            Assert.AreEqual("Unstructured Data", information.UnstructuredRemittanceInformation);
        }
    }
}
