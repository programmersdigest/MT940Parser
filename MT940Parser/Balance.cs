using System;
using System.Text;

namespace programmersdigest.MT940Parser
{
    public class Balance
    {
        public BalanceType Type { get; internal set; }
        public DebitCreditMark Mark { get; internal set; }
        public DateTime? Date { get; internal set; }
        public string Currency { get; internal set; }
        public decimal? Amount { get; internal set; }

        public override string ToString()
        {
            StringBuilder balance = new StringBuilder(250);
            balance.Append($"Type: {Type}{Environment.NewLine}");
            balance.Append($"Date: {Date}{Environment.NewLine}");
            balance.Append($"DebitCreditMark: {Mark}{Environment.NewLine}");
            balance.Append($"Currency: {Currency}{Environment.NewLine}");
            balance.Append($"Amount: {Amount}{Environment.NewLine}");
            return balance.ToString();
        }
    }
}