using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace programmersdigest.MT940Parser.Parsing
{
    internal class AdditionalInfoParser
    {
        private StringReader _reader;
        private char _separator;
        private string _lastRemittanceIdentifier;

        public Information ParseInformation(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value), "Value may not be null");
            }

            _reader = new StringReader(value);
            _separator = default(char);
            _lastRemittanceIdentifier = null;

            var information = new Information();

            DetectFormat(ref information);

            return information;
        }

        private void DetectFormat(ref Information information)
        {
            var value = _reader.Peek(6);
            if (Regex.IsMatch(value, "/[A-Z]{4}/"))
            {
                // Structured format with separators in the form /ABCD/...
                // Cannot read this format at this time.
                ReadUnstructuredData(ref information);
            }
            else if (Regex.IsMatch(value, "[0-9]{3}.*"))
            {
                // Structured format with separators in the form ?12...
                ReadTransactionCode(ref information);
            }
            else
            {
                // Unstructured data or unknown format.
                ReadUnstructuredData(ref information);
            }
        }

        private void ReadTransactionCode(ref Information information)
        {
            var value = _reader.Read(3);
            if (value.Length < 3)
            {
                throw new InvalidDataException($"Expected \"Transaction Code\" with a length of three numeric characters.");
            }

            information.TransactionCode = int.Parse(value);

            if (information.TransactionCode == 999)
            {
                ReadUnstructuredData(ref information);
            }
            else
            {
                // Remove all linebreaks before parsing the data
                value = _reader.Read();
                value = value.Replace("\r\n", "");
                _reader = new StringReader(value);

                DetectSeparator(ref information);
            }
        }

        private void ReadUnstructuredData(ref Information information)
        {
            information.IsUnstructuredData = true;
            information.UnstructuredData = _reader.Read();
        }

        private void DetectSeparator(ref Information information)
        {
            var value = _reader.Read(1);
            if (value.Length < 1)
            {
                throw new InvalidDataException("Unexpected end of statement. Expected \"Separator\"");
            }

            _separator = value[0];      // Some sources say this should always be '?'. Others do however use '@' or other characters.

            DetectFieldCode(ref information);
        }

        private void ReadSeparator(ref Information information)
        {
            var value = _reader.Read(1);
            if (value.Length < 1)
            {
                return;     // End of field contents.
            }

            if (value[0] != _separator)
            {
                throw new InvalidDataException($"Unexpected data \"{value}\". Expected separator \"{_separator}\"");
            }

            DetectFieldCode(ref information);
        }

        private void DetectFieldCode(ref Information information)
        {
            var value = _reader.Read(2);
            if (value.Length < 2)
            {
                throw new InvalidDataException("Unexpected end of statement. Expected \"Field Code\"");
            }

            switch (value)
            {
                case "00":
                    information.PostingText = ReadValue();
                    break;
                case "10":
                    information.JournalNumber = ReadValue();
                    break;
                case "20":
                case "21":
                case "22":
                case "23":
                case "24":
                case "25":
                case "26":
                case "27":
                case "28":
                case "29":
                case "60":
                case "61":
                case "62":
                case "63":
                    ReadRemittanceInformation(ref information);
                    break;
                case "30":
                    information.BankCodeOfPayer = ReadValue();
                    break;
                case "31":
                    information.AccountNumberOfPayer = ReadValue();
                    break;
                case "32":
                case "33":
                    ReadNameOfPayer(ref information);
                    break;
                case "34":
                    ReadTextKeyAddition(ref information);
                    break;
                default:
                    throw new InvalidDataException($"Unknown field code: {value}");
            }

            ReadSeparator(ref information);
        }

        private bool IsFieldCode(string value)
        {
            switch (value)
            {
                case "00":
                case "10":
                case "20":
                case "21":
                case "22":
                case "23":
                case "24":
                case "25":
                case "26":
                case "27":
                case "28":
                case "29":
                case "60":
                case "61":
                case "62":
                case "63":
                case "30":
                case "31":
                case "32":
                case "33":
                case "34":
                    return true;
                default:
                    return false;
            }
        }

        private void ReadNameOfPayer(ref Information information)
        {
            information.NameOfPayer += ReadValue();
        }

        private void ReadTextKeyAddition(ref Information information)
        {
            var value = ReadValue();
            information.TextKeyAddition = int.Parse(value);
        }

        private string ReadValue()
        {
            var result = new StringBuilder();
            bool foundFieldCode = false;

            do
            {
                var readString = _reader.ReadWhile(c => c != _separator);
                result.Append(readString);

                var nextChars = _reader.Peek(3);
                foundFieldCode = nextChars.Length < 3 || IsFieldCode(nextChars.Substring(1));
                if (!foundFieldCode)
                {
                    result.Append(_reader.Read(1));
                }
            } while (!foundFieldCode);

            return result.ToString();
        }

        private void ReadRemittanceInformation(ref Information information)
        {
            var value = _reader.Read(5);
            if (!DetectRemittanceIdentifier(value, ref information))
            {
                _reader.Skip(-value.Length);   // Revert read

                // Could not detect identifier. Try to append to last one.
                if (_lastRemittanceIdentifier != null)
                {
                    DetectRemittanceIdentifier(_lastRemittanceIdentifier, ref information);
                }
                else
                {
                    // There is no last remittance identifier, regard as unstructured data.
                    if (!string.IsNullOrEmpty(information.UnstructuredRemittanceInformation))
                    {
                        information.UnstructuredRemittanceInformation += "\r\n";        // Add newline to make remittance information more readable
                    }

                    information.UnstructuredRemittanceInformation += ReadValue();
                }
            }
        }

        private bool DetectRemittanceIdentifier(string identifier, ref Information information)
        {
            switch (identifier)
            {
                case "EREF+":
                    information.EndToEndReference += ReadValue();
                    break;
                case "KREF+":
                    information.CustomerReference += ReadValue();
                    break;
                case "MREF+":
                    information.MandateReference += ReadValue();
                    break;
                case "CRED+":
                    information.CreditorReference += ReadValue();
                    break;
                case "DEBT+":
                    information.OriginatorsIdentificationCode += ReadValue();
                    break;
                case "COAM+":
                    information.CompensationAmount += ReadValue();
                    break;
                case "OAMT+":
                    information.OriginalAmount += ReadValue();
                    break;
                case "SVWZ+":
                    if (!string.IsNullOrEmpty(information.SepaRemittanceInformation))
                    {
                        information.SepaRemittanceInformation += "\r\n";        // Add newline to make remittance information more readable
                    }

                    information.SepaRemittanceInformation += ReadValue();
                    break;
                case "ABWA+":
                    information.PayersReferenceParty += ReadValue();
                    break;
                case "ABWE+":
                    information.CreditorsReferenceParty += ReadValue();
                    break;
                default:
                    return false;
            }

            _lastRemittanceIdentifier = identifier;
            return true;
        }
    }
}
