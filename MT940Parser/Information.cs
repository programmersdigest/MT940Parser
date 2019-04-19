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
    }
}