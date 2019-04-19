using System.Collections.Generic;

namespace programmersdigest.MT940Parser
{
    public class Statement
    {
        public string TransactionReferenceNumber { get; internal set; }
        public string RelatedReference { get; internal set; }
        public string AccountIdentification { get; internal set; }
        public long? StatementNumber { get; internal set; }
        public long? SequenceNumber { get; internal set; }
        public Balance OpeningBalance { get; internal set; }
        public Balance ClosingBalance { get; internal set; }
        public Balance ClosingAvailableBalance { get; internal set; }
        public Information InformationToOwner { get; internal set; }
        public List<StatementLine> Lines { get; } = new List<StatementLine>();
        public List<Balance> ForwardAvailableBalances { get; } = new List<Balance>();
    }
}
