using System;
using System.Text;

namespace programmersdigest.MT940Parser
{
    public class Information
    {
        public int TransactionCode { get; internal set; }
        public string PostingText { get; internal set; }
        public string JournalNumber { get; internal set; }
        public string BankCodeOfPayer { get; internal set; }
        public string AccountNumberOfPayer { get; internal set; }
        public string NameOfPayer { get; internal set; }
        public int? TextKeyAddition { get; internal set; }
        public string EndToEndReference { get; internal set; }
        public string CustomerReference { get; internal set; }
        public string MandateReference { get; internal set; }
        public string CreditorReference { get; internal set; }
        public string OriginatorsIdentificationCode { get; internal set; }
        public string CompensationAmount { get; internal set; }
        public string OriginalAmount { get; internal set; }
        public string SepaRemittanceInformation { get; internal set; }
        public string PayersReferenceParty { get; internal set; }
        public string CreditorsReferenceParty { get; internal set; }
        public string UnstructuredRemittanceInformation { get; internal set; }
        public bool IsUnstructuredData { get; internal set; }
        public string UnstructuredData { get; internal set; }

        public override string ToString()
        {

            StringBuilder info = new StringBuilder(500);
            info.Append($"TransactionCode: {TransactionCode}{Environment.NewLine}");
            info.Append($"PostingText: {PostingText}{Environment.NewLine}");
            info.Append($"JournalNumber: {JournalNumber}{Environment.NewLine}");
            info.Append($"BankCodeOfPayer: {BankCodeOfPayer}{Environment.NewLine}");
            info.Append($"AccountNumberOfPayer: {AccountNumberOfPayer}{Environment.NewLine}");
            info.Append($"NameOfPayer: {NameOfPayer}{Environment.NewLine}");
            info.Append($"TextKeyAddition: {TextKeyAddition}{Environment.NewLine}");
            info.Append($"EndToEndReference: {EndToEndReference}{Environment.NewLine}");
            info.Append($"CustomerReference: {CustomerReference}{Environment.NewLine}");
            info.Append($"MandateReference: {MandateReference}{Environment.NewLine}");
            info.Append($"CreditorReference: {CreditorReference}{Environment.NewLine}");
            info.Append($"OriginatorsIdentificationCode: {OriginatorsIdentificationCode}{Environment.NewLine}");
            info.Append($"CompensationAmount: {CompensationAmount}{Environment.NewLine}");
            info.Append($"OriginalAmount: {OriginalAmount}{Environment.NewLine}");
            info.Append($"SepaRemittanceInformation: {SepaRemittanceInformation}{Environment.NewLine}");
            info.Append($"PayersReferenceParty: {PayersReferenceParty}{Environment.NewLine}");
            info.Append($"CreditorsReferenceParty: {CreditorsReferenceParty}{Environment.NewLine}");
            info.Append($"UnstructuredRemittanceInformation: {UnstructuredRemittanceInformation}{Environment.NewLine}");
            info.Append($"IsUnstructuredData: {IsUnstructuredData}{Environment.NewLine}");
            info.Append($"UnstructuredData: {UnstructuredData}{Environment.NewLine}");

            return info.ToString();
        }
    }
}