using System;
using System.IO;
using System.Linq;

namespace programmersdigest.MT940Parser.Parsing
{
    public class StatementParser
    {
        private StreamReader _reader;
        private readonly BalanceParser _balanceParser = new BalanceParser();
        private readonly StatementLineParser _statementLineParser = new StatementLineParser();
        private readonly AdditionalInfoParser _additionalInfoParser = new AdditionalInfoParser();

        public StatementParser(StreamReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            _reader = reader;
        }

        public Statement ReadStatement()
        {
            var statement = new Statement();

            _reader.Find("\r\n:20:");
            if (_reader.EndOfStream)
            {
                return null;
            }

            ReadTransactionReferenceNumber(ref statement);
            return statement;
        }

        private void ReadTransactionReferenceNumber(ref Statement statement)
        {
            var value = _reader.ReadTo(out var nextKey, "\r\n:21:", "\r\n:25:");

            switch (nextKey)
            {
                case "\r\n:21:":
                    ReadRelatedReference(ref statement);
                    break;
                case "\r\n:25:":
                    ReadAccountIdentification(ref statement);
                    break;
                default:
                    throw new InvalidDataException("The statement data ended unexpectedly. Expected field :20: to be followed by :21: or :25:");
            }

            statement.TransactionReferenceNumber = value;
        }

        private void ReadRelatedReference(ref Statement statement)
        {
            var value = _reader.ReadTo(out var nextKey, "\r\n:25:");
            if (nextKey == null)
            {
                throw new InvalidDataException("The statement data ended unexpectedly. Expected field :21: to be followed by :25:");
            }

            statement.RelatedReference = value;

            ReadAccountIdentification(ref statement);
        }

        private void ReadAccountIdentification(ref Statement statement)
        {
            var value = _reader.ReadTo(out var nextKey, "\r\n:28C:");
            if (nextKey == null)
            {
                throw new InvalidDataException("The statement data ended unexpectedly. Expected field :25: to be followed by :28C:");
            }

            statement.AccountIdentification = value;

            ReadStatementNumber(ref statement);
        }

        private void ReadStatementNumber(ref Statement statement)
        {
            var value = _reader.ReadTo(out var nextKey, "/", "\r\n:60F:", "\r\n:60M:");

            switch (nextKey)
            {
                case "/":
                    ReadSequenceNumber(ref statement);
                    break;
                case "\r\n:60F:":
                    ReadOpeningBalance(ref statement, BalanceType.Opening);
                    break;
                case "\r\n:60M:":
                    ReadOpeningBalance(ref statement, BalanceType.Intermediate);
                    break;
                default:
                    throw new InvalidDataException("The statement data ended unexpectedly. Expected field :28C: to be followed by :60F: or :60M:");
            }

            statement.StatementNumber = long.Parse(value);
        }

        private void ReadSequenceNumber(ref Statement statement)
        {
            var value = _reader.ReadTo(out var nextKey, "\r\n:60F:", "\r\n:60M:");

            switch (nextKey)
            {
                case "\r\n:60F:":
                    ReadOpeningBalance(ref statement, BalanceType.Opening);
                    break;
                case "\r\n:60M:":
                    ReadOpeningBalance(ref statement, BalanceType.Intermediate);
                    break;
                default:
                    throw new InvalidDataException("The statement data ended unexpectedly. Expected field :28C: to be followed by :60F: or :60M:");
            }

            statement.SequenceNumber = long.Parse(value);
        }

        private void ReadOpeningBalance(ref Statement statement, BalanceType balanceType)
        {
            var value = _reader.ReadTo(out var nextKey, "\r\n:61:", "\r\n:62F:", "\r\n:62M:", "\r\n:86:");
            switch (nextKey)
            {
                case "\r\n:61:":
                    ReadStatementLine(ref statement);
                    break;
                case "\r\n:62F:":
                    ReadClosingBalance(ref statement, BalanceType.Closing);
                    break;
                case "\r\n:62M:":
                    ReadClosingBalance(ref statement, BalanceType.Intermediate);
                    break;
                case "\r\n:86:":
                    throw new InvalidDataException("The statement :86: should be preceeded by :61:");
                default:
                    throw new InvalidDataException("The statement data ended unexpectedly. Expected field :60a: to be followed by :61:, :62F: or :62M:");
            }

            statement.OpeningBalance = _balanceParser.ReadBalance(value, balanceType);
        }

        private void ReadStatementLine(ref Statement statement)
        {
            var value = _reader.ReadTo(out var nextKey, "\r\n:61:", "\r\n:62F:", "\r\n:62M:", "\r\n:86:");

            // Check the format and parse the statement line to keep correct line ordering.
            // If we were to parse the line after the switch, lines would be in reversed order.
            if (nextKey == null)
            {
                throw new InvalidDataException("The statement data ended unexpectedly. Expected field :61: to be followed by :61:, :62F:, :62M: or :86:");
            }

            var statementLine = _statementLineParser.ReadStatementLine(value);
            statement.Lines.Add(statementLine);

            switch (nextKey)
            {
                case "\r\n:61:":
                    ReadStatementLine(ref statement);
                    break;
                case "\r\n:62F:":
                    ReadClosingBalance(ref statement, BalanceType.Closing);
                    break;
                case "\r\n:62M:":
                    ReadClosingBalance(ref statement, BalanceType.Intermediate);
                    break;
                case "\r\n:86:":
                    ReadLineInformationToOwner(ref statement);
                    break;
                default:
                    throw new InvalidDataException("The statement data ended unexpectedly. Expected field :61: to be followed by :61:, :62F:, :62M: or :86:");
            }
        }

        private void ReadLineInformationToOwner(ref Statement statement)
        {
            var value = _reader.ReadTo(out var nextKey, "\r\n:61:", "\r\n:62F:", "\r\n:62M:");
            if (nextKey == null)
            {
                throw new InvalidDataException("The statement data ended unexpectedly. Expected field :86: to be followed by :61:, :62F: or :62M:");
            }

            var lastLine = statement.Lines.LastOrDefault();
            if (lastLine == null)
            {
                throw new FormatException($"Expecting field :86: to be preceeded by field :61:");
            }

            lastLine.InformationToOwner = _additionalInfoParser.ParseInformation(value);

            switch (nextKey)
            {
                case "\r\n:61:":
                    ReadStatementLine(ref statement);
                    break;
                case "\r\n:62F:":
                    ReadClosingBalance(ref statement, BalanceType.Closing);
                    break;
                case "\r\n:62M:":
                    ReadClosingBalance(ref statement, BalanceType.Intermediate);
                    break;
                default:
                    throw new InvalidDataException("The statement data ended unexpectedly. Expected field :86: to be followed by :61:, :62F: or :62M:");
            }
        }

        private void ReadClosingBalance(ref Statement statement, BalanceType balanceType)
        {
            var value = _reader.ReadTo(out var nextKey, "\r\n:64:", "\r\n:65:", "\r\n:86:", "\r\n-");
            switch (nextKey)
            {
                case "\r\n:64:":
                    ReadClosingAvailableBalance(ref statement);
                    break;
                case "\r\n:65:":
                    ReadForwardAvailableBalance(ref statement);
                    break;
                case "\r\n:86:":
                    ReadStatementInformationToOwner(ref statement);
                    break;
                case "\r\n-":
                    break;      // End of statement
                default:
                    throw new InvalidDataException("The statement data ended unexpectedly. Expected field :62a: to be followed by :64:, :65:, :86: or the end of the statement");
            }

            statement.ClosingBalance = _balanceParser.ReadBalance(value, balanceType);
        }

        private void ReadClosingAvailableBalance(ref Statement statement)
        {
            var value = _reader.ReadTo(out var nextKey, "\r\n:65:", "\r\n:86:", "\r\n-");
            switch (nextKey)
            {
                case "\r\n:65:":
                    ReadForwardAvailableBalance(ref statement);
                    break;
                case "\r\n:86:":
                    ReadStatementInformationToOwner(ref statement);
                    break;
                case "\r\n-":
                    break;      // End of statement
                default:
                    throw new InvalidDataException("The statement data ended unexpectedly. Expected field :64: to be followed by :65:, :86: or the end of the statement");
            }

            statement.ClosingAvailableBalance = _balanceParser.ReadBalance(value, BalanceType.None);
        }

        private void ReadForwardAvailableBalance(ref Statement statement)
        {
            var value = _reader.ReadTo(out var nextKey, "\r\n:65:", "\r\n:86:", "\r\n-");
            if (nextKey == null)
            {
                throw new InvalidDataException("The statement data ended unexpectedly. Expected field :65: to be followed by :65:, :86: or the end of the statement");
            }

            var balance = _balanceParser.ReadBalance(value, BalanceType.None);
            statement.ForwardAvailableBalances.Add(balance);

            switch (nextKey)
            {
                case "\r\n:65:":
                    ReadForwardAvailableBalance(ref statement);
                    break;
                case "\r\n:86:":
                    ReadStatementInformationToOwner(ref statement);
                    break;
                case "\r\n-":
                    break;      // End of statement
                default:
                    throw new InvalidDataException("The statement data ended unexpectedly. Expected field :65: to be followed by :65:, :86: or the end of the statement");
            }
        }

        private void ReadStatementInformationToOwner(ref Statement statement)
        {
            var value = _reader.ReadTo(out var nextKey, "\r\n-");
            if (nextKey == null)
            {
                throw new InvalidDataException("The statement data ended unexpectedly. Expected field :86: to be followed by the end of the statement");
            }

            statement.InformationToOwner = _additionalInfoParser.ParseInformation(value);
        }
    }
}
