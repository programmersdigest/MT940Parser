using System;
using System.Globalization;
using System.IO;

namespace programmersdigest.MT940Parser.Parsing
{
    public class BalanceParser
    {
        private StringReader _reader;

        public Balance ReadBalance(string buffer, BalanceType type)
        {
            _reader = new StringReader(buffer);

            var balance = new Balance
            {
                Type = type
            };

            ReadDebitCreditMark(ref balance);

            return balance;
        }

        private void ReadDebitCreditMark(ref Balance balance)
        {
            var value = _reader.Read(1);
            if (value.Length < 1)
            {
                throw new InvalidDataException("The balance data ended unexpectedly. Expected credit debit field.");
            }

            switch (value)
            {
                case "C":
                    balance.Mark = DebitCreditMark.Credit;
                    break;
                case "D":
                    balance.Mark = DebitCreditMark.Debit;
                    break;
                default:
                    throw new FormatException($"Debit/Credit Mark must be 'C' or 'D'. Actual: {value}");
            }

            ReadStatementDate(ref balance);
        }

        private void ReadStatementDate(ref Balance balance)
        {
            var value = _reader.ReadWhile(char.IsDigit, 6);
            if (value.Length < 6)
            {
                throw new InvalidDataException("The balance data ended unexpectedly. Expected value Date with a length of six characters.");
            }

            balance.Date = DateParser.Parse(value);

            ReadCurrency(ref balance);
        }

        private void ReadCurrency(ref Balance balance)
        {
            var value = _reader.ReadWhile(c => char.IsLetter(c) && char.IsUpper(c), 3);
            if (value.Length < 3)
            {
                throw new InvalidDataException("The balance data ended unexpectedly. Expected value Currency with three uppercase letters.");
            }

            balance.Currency = value;

            ReadAmount(ref balance);
        }

        private void ReadAmount(ref Balance balance)
        {
            var value = _reader.Read(15);
            if (value.Length <= 0)
            {
                throw new InvalidDataException("The balance data ended unexpectedly. Expected value Amount with a length of at least 1 decimal.");
            }

            if (!decimal.TryParse(value, NumberStyles.AllowDecimalPoint, CultureInfo.GetCultureInfo("de"), out var amount))
            {
                throw new FormatException($"Cannot convert value to Decimal: {value}");
            }

            balance.Amount = amount;
        }
    }
}
