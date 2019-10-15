using System;
using System.Text;

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

        public override string ToString()
        {
            StringBuilder line = new StringBuilder(250);
            line.Append($"ValueDate: {ValueDate}{Environment.NewLine}");
            line.Append($"EntryDate: {EntryDate}{Environment.NewLine}");
            line.Append($"Mark: {Mark}{Environment.NewLine}");
            line.Append($"FundsCode: {FundsCode}{Environment.NewLine}");
            line.Append($"Amount: {Amount}{Environment.NewLine}");
            line.Append($"TransactionTypeIdCode: {TransactionTypeIdCode}{Environment.NewLine}");
            line.Append($"CustomerReference: {CustomerReference}{Environment.NewLine}");
            line.Append($"SupplementaryDetails: {SupplementaryDetails}{Environment.NewLine}");
            line.Append($"BankReference: {BankReference}{Environment.NewLine}");
            line.Append($"InformationToOwner: {InformationToOwner}{Environment.NewLine}");

            return line.ToString();
        }
    }
}