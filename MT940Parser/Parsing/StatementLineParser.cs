using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace programmersdigest.MT940Parser.Parsing
{
    public class StatementLineParser
    {
        private static readonly HashSet<string> _validTransactionTypes = new HashSet<string> { "F", "N", "S" };

        private StringReader _reader;

        public StatementLine ReadStatementLine(string buffer)
        {
            _reader = new StringReader(buffer);

            var statementLine = new StatementLine();

            ReadValueDate(ref statementLine);

            var leftover = _reader.Peek();
            if (!string.IsNullOrEmpty(leftover))
            {
                throw new InvalidDataException($"Expected end of statement line. Data not read: {leftover}");
            }

            return statementLine;
        }

        private void ReadValueDate(ref StatementLine statementLine)
        {
            var value = _reader.Read(6);
            if (value.Length < 6)
            {
                throw new InvalidDataException("The statement line data ended unexpectedly. Expected \"Value Date\" with a length of six characters.");
            }

            statementLine.ValueDate = DateParser.Parse(value);

            ReadEntryDate(ref statementLine);
        }

        private void ReadEntryDate(ref StatementLine statementLine)
        {
            var value = _reader.ReadWhile(c => char.IsNumber(c), 4);
            if (value.Length > 0)
            {
                if (value.Length < 4)
                {
                    throw new InvalidDataException("The statement line data ended unexpectedly. Detected field \"Entry Date\", however the field does not have the expected four characters.");
                }

                var valueDateYear = statementLine.ValueDate.Value.ToString("yy");
                value = $"{valueDateYear}{value}";

                var date = DateParser.Parse(value);

                if (date > statementLine.ValueDate)
                {
                    date = date.AddYears(-1);  // Correct entry date if new year has happened between entry and value date.
                }

                statementLine.EntryDate = date;
            }

            ReadDebitCreditMark(ref statementLine);
        }

        private void ReadDebitCreditMark(ref StatementLine statementLine)
        {
            var value = _reader.Read(1);
            if (value.Length < 1)
            {
                throw new InvalidDataException("The statement line data ended unexpectedly. Expected credit debit field.");
            }

            if (value == "R")
            {
                value += _reader.Read(1);   // Two character field - read next char
                if (value.Length < 2)
                {
                    throw new InvalidDataException("The statement line data ended unexpectedly. Expected credit debit field with two characters.");
                }
            }

            switch (value)
            {
                case "C":
                    statementLine.Mark = DebitCreditMark.Credit;
                    break;
                case "D":
                    statementLine.Mark = DebitCreditMark.Debit;
                    break;
                case "RC":
                    statementLine.Mark = DebitCreditMark.ReverseCredit;
                    break;
                case "RD":
                    statementLine.Mark = DebitCreditMark.ReverseDebit;
                    break;
                default:
                    throw new FormatException($"Debit/Credit Mark must be 'C', 'D', 'RC' or 'RD'. Actual: {value}");
            }

            ReadFundsCode(ref statementLine);
        }

        private void ReadFundsCode(ref StatementLine statementLine)
        {
            var value = _reader.ReadWhile(c => char.IsLetter(c), 1);
            if (value.Length >= 1)
            {
                statementLine.FundsCode = value[0];
            }

            ReadAmount(ref statementLine);
        }

        private void ReadAmount(ref StatementLine statementLine)
        {
            var value = _reader.ReadWhile(c => c == ',' || char.IsNumber(c), 15);
            if (value.Length < 1)
            {
                throw new InvalidDataException("The statement line data ended unexpectedly. Expected \"Amount\" with a length of at least 1 decimal.");
            }

            if (!decimal.TryParse(value, NumberStyles.AllowDecimalPoint, CultureInfo.GetCultureInfo("de"), out var amount))
            {
                throw new FormatException($"Cannot convert value to Decimal: {value}");
            }

            statementLine.Amount = amount;

            ReadTransactionTypeIdCode(ref statementLine);
        }

        private void ReadTransactionTypeIdCode(ref StatementLine statementLine)
        {
            var constant = _reader.Read(1);

            if (!(_validTransactionTypes.Contains(constant)))
            {
                throw new InvalidDataException("The statement line contained unexpected data. Expected constant \"N, S Or F\" before \"Transaction Type ID Code\".");
            }

            // TODO Make enum with values from "DFÜ - Abkommen S. 436"
            var value = _reader.Read(3);
            if (value.Length < 3)
            {
                throw new InvalidDataException("The statement line data ended unexpectedly. Expected \"Transaction Type ID Code\" with a length of three characters.");
            }

            statementLine.TransactionTypeIdCode = value;

            ReadCustomerReference(ref statementLine);
        }

        private void ReadCustomerReference(ref StatementLine statementLine)
        {
            var value = _reader.Peek(18);   // 16x + either "//" or "\r\n"
            if (value.Contains("//"))
            {
                // value contains beginning of bank reference.
                // Remove it and read bank reference.
                var idx = value.IndexOf("//");
                value = _reader.Read(idx);

                ReadBankReference(ref statementLine);
            }
            else if (value.Contains("\r\n"))
            {
                // value contains beginning of supplementary details.
                // Remove it and read supplementary details.
                var idx = value.IndexOf("\r\n");
                value = _reader.Read(idx);

                ReadSupplementaryDetails(ref statementLine);
            }
            else
            {
                _reader.Skip(16);
            }

            if (value.Length < 1)
            {
                throw new InvalidDataException("The statement line data ended unexpectedly. Expected \"Customer Reference\" with a length of at least 1 character.");
            }

            if (value.Length > 16)
            {
                value = value.Substring(0, 16);
            }
            statementLine.CustomerReference = value;
        }

        private void ReadBankReference(ref StatementLine statementLine)
        {
            var value = _reader.Peek(20);   // "//" + 16x + "\r\n"
            if (value.Length <= 0)
            {
                return;     // End of buffer
            }

            if (!value.StartsWith("//"))
            {
                throw new InvalidDataException($"Unexpected data found. Expected \"Bank Reference\". Actual {value}");
            }

            if (value.Contains("\r\n"))
            {
                // value contains beginning of supplementary details - remove it.
                var idx = value.IndexOf("\r\n");
                value = _reader.Read(idx);

                ReadSupplementaryDetails(ref statementLine);
            }
            else
            {
                _reader.Skip(18);
            }

            value = value.Substring(2);

            if (value.Length > 16)
            {
                value = value.Substring(0, 16);
            }

            statementLine.BankReference = value;
        }

        private void ReadSupplementaryDetails(ref StatementLine statementLine)
        {
            var value = _reader.Read(36);   // 34 + "\r\n"
            if (value.Length <= 0)
            {
                return;     // End of buffer
            }

            if (!value.StartsWith("\r\n"))
            {
                throw new InvalidDataException($"Unexpected data found. Expected \"Supplementary Details\". Actual {value}");
            }

            value = value.Substring(2);
            statementLine.SupplementaryDetails = value;
        }
    }
}
