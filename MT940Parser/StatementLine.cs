using System;

namespace programmersdigest.MT940Parser
{
    public class StatementLine
    {
        public DateTime? ValueDate { get; internal set; }
        public DateTime? EntryDate { get; internal set; }
        public DebitCreditMark Mark { get; internal set; }
        public char? FundsCode { get; internal set; }
        public decimal? Amount { get; internal set; }
        public string TransactionTypeIdCode { get; internal set; }
        public string CustomerReference { get; internal set; }
        public string SupplementaryDetails { get; internal set; }
        public string BankReference { get; internal set; }
        public Information InformationToOwner { get; internal set; }
    }
}