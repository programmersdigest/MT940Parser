using Microsoft.VisualStudio.TestTools.UnitTesting;
using programmersdigest.MT940Parser;
using programmersdigest.MT940Parser.Parsing;
using System;
using System.IO;
using System.Text;

namespace MT940ParserTests
{
    [TestClass]
    public class StatementParserTests
    {
        #region Input checks

        [TestMethod]
        public void StatementParser_InputIsEmpty_ShouldReturnNull()
        {
            var text = "";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.IsNull(statement);
            }
        }

        [TestMethod]
        public void StatementParser_InputIsNonsensical_ShouldReturnNull()
        {
            var text = "32u1mbﬂ61zﬂv";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.IsNull(statement);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StatementParser_ReaderIsNull_ShouldThrowArgumentNullException()
        {
            StreamReader reader = null;
            var parser = new StatementParser(reader);
        }

        #endregion

        #region Mandatory and optional fields

        [TestMethod]
        public void StatementParser_SingleCompleteStatement_ShouldParse()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/1234567890\r\n" +
                ":28C:12345/12345\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":62F:C180220EUR0,00\r\n" +
                ":64:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":86:Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.IsNotNull(statement);
            }
        }

        [TestMethod]
        public void StatementParser_MissingStartingField20_ShouldParseNothing()
        {
            var text =
                ":21:Related Ref\r\n" +
                ":25:1234567890/1234567890\r\n" +
                ":28C:12345/12345\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":62F:C180220EUR0,00\r\n" +
                ":64:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":86:Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.IsNull(statement);
            }
        }

        [TestMethod]
        public void StatementParser_MissingOptionalField21_ShouldParse()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":25:1234567890/1234567890\r\n" +
                ":28C:12345/12345\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":62F:C180220EUR0,00\r\n" +
                ":64:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":86:Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.IsNotNull(statement);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void StatementParser_MissingMandatoryField25_ShouldThrowInvalidDataException()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":28C:12345/12345\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":62F:C180220EUR0,00\r\n" +
                ":64:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":86:Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void StatementParser_MissingMandatoryField28C_ShouldThrowInvalidDataException()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/1234567890\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":62F:C180220EUR0,00\r\n" +
                ":64:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":86:Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();
            }
        }

        [TestMethod]
        public void StatementParser_MissingOptionalSecondPartInField28C_ShouldParse()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/1234567890\r\n" +
                ":28C:12345\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":62F:C180220EUR0,00\r\n" +
                ":64:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":86:Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.IsNotNull(statement);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void StatementParser_MissingMandatoryField60a_ShouldThrowInvalidDataException()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/1234567890\r\n" +
                ":28C:12345/12345\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":62F:C180220EUR0,00\r\n" +
                ":64:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":86:Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void StatementParser_MissingField61Before86_ShouldThrowInvalidDataException()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/1234567890\r\n" +
                ":28C:12345/12345\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":86:Information\r\n" +
                ":86:Information\r\n" +
                ":62F:C180220EUR0,00\r\n" +
                ":64:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":86:Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.IsNotNull(statement);
            }
        }

        [TestMethod]
        public void StatementParser_MissingOptionalField61And86_ShouldParse()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/1234567890\r\n" +
                ":28C:12345/12345\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":62F:C180220EUR0,00\r\n" +
                ":64:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":86:Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.IsNotNull(statement);
            }
        }

        [TestMethod]
        public void StatementParser_MissingOptionalField86_ShouldParse()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/1234567890\r\n" +
                ":28C:12345/12345\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":62F:C180220EUR0,00\r\n" +
                ":64:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":86:Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.IsNotNull(statement);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void StatementParser_MissingMandatoryField62a_ShouldThrowInvalidDataException()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/1234567890\r\n" +
                ":28C:12345/12345\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":64:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":86:Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();
            }
        }

        [TestMethod]
        public void StatementParser_MissingOptionalField64_ShouldParse()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/1234567890\r\n" +
                ":28C:12345/12345\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":62F:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":86:Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.IsNotNull(statement);
            }
        }

        [TestMethod]
        public void StatementParser_MissingOptionalField65_ShouldParse()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/1234567890\r\n" +
                ":28C:12345/12345\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":62F:C180220EUR0,00\r\n" +
                ":64:C180220EUR0,00\r\n" +
                ":86:Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.IsNotNull(statement);
            }
        }

        [TestMethod]
        public void StatementParser_MissingOptionalField86AtEndOfStatement_ShouldParse()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/1234567890\r\n" +
                ":28C:12345/12345\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":62F:C180220EUR0,00\r\n" +
                ":64:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.IsNotNull(statement);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void StatementParser_MissingEndOfStatement_ShouldThrowInvalidDataException()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/1234567890\r\n" +
                ":28C:12345/12345\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":62F:C180220EUR0,00\r\n" +
                ":64:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":86:Information";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();
            }
        }

        #endregion

        #region Field length

        [TestMethod]
        public void StatementParser_Field20TooLong_ShouldParse()
        {
            var text = "\r\n" +
                ":20:" + new string('x', 16 + 1) + "\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/1234567890\r\n" +
                ":28C:12345/12345\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":62F:C180220EUR0,00\r\n" +
                ":64:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":86:Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.AreEqual(new string('x', 16 + 1), statement.TransactionReferenceNumber);
            }
        }

        [TestMethod]
        public void StatementParser_Field21TooLong_ShouldParse()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:" + new string('x', 16 + 1) + "\r\n" +
                ":25:1234567890/1234567890\r\n" +
                ":28C:12345/12345\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":62F:C180220EUR0,00\r\n" +
                ":64:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":86:Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.AreEqual(new string('x', 16 + 1), statement.RelatedReference);
            }
        }

        [TestMethod]
        public void StatementParser_Field25TooLong_ShouldParse()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:" + new string('x', 35 + 1) + "\r\n" +
                ":28C:12345/12345\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":62F:C180220EUR0,00\r\n" +
                ":64:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":86:Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.AreEqual(new string('x', 35 + 1), statement.AccountIdentification);
            }
        }

        [TestMethod]
        public void StatementParser_Field28CFirstPartTooLong_ShouldParse()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/1234567890\r\n" +
                ":28C:" + new string('1', 5 + 1) + "/12345\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":62F:C180220EUR0,00\r\n" +
                ":64:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":86:Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.AreEqual(111111, statement.StatementNumber);
            }
        }

        [TestMethod]
        public void StatementParser_Field28CSecondPartTooLong_ShouldParse()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/1234567890\r\n" +
                ":28C:12345/" + new string('1', 5 + 1) + "\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":62F:C180220EUR0,00\r\n" +
                ":64:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":86:Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.AreEqual(111111, statement.SequenceNumber);
            }
        }

        [TestMethod]
        public void StatementParser_Field86InLineTooLong_ShouldParse()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/1234567890\r\n" +
                ":28C:12345/12345\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:" + new string('x', 6 * 65 + 1) + "\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":62F:C180220EUR0,00\r\n" +
                ":64:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":86:Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.AreEqual(new string('x', 6 * 65 + 1), statement.Lines[0].InformationToOwner.UnstructuredData);
            }
        }

        [TestMethod]
        public void StatementParser_Field86InLineTooManyLines_ShouldParse()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/1234567890\r\n" +
                ":28C:12345/12345\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:1\r\n" +
                "2\r\n" +
                "3\r\n" +
                "4\r\n" +
                "5\r\n" +
                "6\r\n" +
                "7\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":62F:C180220EUR0,00\r\n" +
                ":64:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":86:Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.AreEqual(
                    "1\r\n" +
                    "2\r\n" +
                    "3\r\n" +
                    "4\r\n" +
                    "5\r\n" +
                    "6\r\n" +
                    "7",
                    statement.Lines[0].InformationToOwner.UnstructuredData);
            }
        }

        [TestMethod]
        public void StatementParser_Field86AtEndOfStatementTooLong_ShouldParse()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/1234567890\r\n" +
                ":28C:12345/12345\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":62F:C180220EUR0,00\r\n" +
                ":64:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":86:" + new string('x', 6 * 65 + 1) + "\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.AreEqual(new string('x', 6 * 65 + 1), statement.InformationToOwner.UnstructuredData);
            }
        }

        [TestMethod]
        public void StatementParser_Field86AtEndOfStatentTooManyLines_ShouldParse()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/1234567890\r\n" +
                ":28C:12345/12345\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":62F:C180220EUR0,00\r\n" +
                ":64:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":86:1\r\n" +
                "2\r\n" +
                "3\r\n" +
                "4\r\n" +
                "5\r\n" +
                "6\r\n" +
                "7\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.AreEqual(
                    "1\r\n" +
                    "2\r\n" +
                    "3\r\n" +
                    "4\r\n" +
                    "5\r\n" +
                    "6\r\n" +
                    "7",
                    statement.InformationToOwner.UnstructuredData);
            }
        }

        #endregion

        #region Empty fields

        [TestMethod]
        public void StatementParser_Field20IsEmpty_ShouldParse()
        {
            var text = "\r\n" +
                ":20:\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/1234567890\r\n" +
                ":28C:12345/12345\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":62F:C180220EUR0,00\r\n" +
                ":64:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":86:Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.IsNotNull(statement);
            }
        }

        [TestMethod]
        public void StatementParser_Field21IsEmpty_ShouldParse()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:\r\n" +
                ":25:1234567890/1234567890\r\n" +
                ":28C:12345/12345\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":62F:C180220EUR0,00\r\n" +
                ":64:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":86:Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.IsNotNull(statement);
            }
        }

        [TestMethod]
        public void StatementParser_Field25IsEmpty_ShouldParse()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:\r\n" +
                ":28C:12345/12345\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":62F:C180220EUR0,00\r\n" +
                ":64:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":86:Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.IsNotNull(statement);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void StatementParser_Field28Part1IsEmpty_ShouldThrowFormatException()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/1234567890\r\n" +
                ":28C:/12345\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":62F:C180220EUR0,00\r\n" +
                ":64:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":86:Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void StatementParser_Field28Part2IsEmpty_ShouldThrowFormatException()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/1234567890\r\n" +
                ":28C:12345/\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":62F:C180220EUR0,00\r\n" +
                ":64:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":86:Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();
            }
        }

        [TestMethod]
        public void StatementParser_Field86InLineIsEmpty_ShouldParse()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/1234567890\r\n" +
                ":28C:12345/12345\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":62F:C180220EUR0,00\r\n" +
                ":64:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":86:Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.IsNotNull(statement);
            }
        }

        [TestMethod]
        public void StatementParser_Field86AtEndOfStatementIsEmpty_ShouldParse()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/1234567890\r\n" +
                ":28C:12345/12345\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":61:1802200220CR0,00NMSCCustomer Ref//Bank Ref\r\n" +
                "Supplementary Details\r\n" +
                ":86:Information\r\n" +
                ":62F:C180220EUR0,00\r\n" +
                ":64:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":65:C180220EUR0,00\r\n" +
                ":86:\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.IsNotNull(statement);
            }
        }

        #endregion

        #region Basic positive result checks

        [TestMethod]
        public void StatementParser_SingleCompleteStatement_ShouldParseField20()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/0987654321\r\n" +
                ":28C:12345/54321\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802220221CR1,00NRBTCustomer Ref 1//Bank Ref 1\r\n" +
                "Supplementary Details 1\r\n" +
                ":86:Line Information 1\r\n" +
                ":61:1802240223DF2,00NMSCCustomer Ref 2//Bank Ref 2\r\n" +
                "Supplementary Details 2\r\n" +
                ":86:Line Information 2\r\n" +
                ":62F:C180225EUR3,00\r\n" +
                ":64:D180226EUR4,00\r\n" +
                ":65:C180227EUR5,00\r\n" +
                ":65:D180228EUR6,00\r\n" +
                ":86:Statement Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.AreEqual("Transaction Ref", statement.TransactionReferenceNumber);
            }
        }

        [TestMethod]
        public void StatementParser_SingleCompleteStatement_ShouldParseField21()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/0987654321\r\n" +
                ":28C:12345/54321\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802220221CR1,00NRBTCustomer Ref 1//Bank Ref 1\r\n" +
                "Supplementary Details 1\r\n" +
                ":86:Line Information 1\r\n" +
                ":61:1802240223DF2,00NMSCCustomer Ref 2//Bank Ref 2\r\n" +
                "Supplementary Details 2\r\n" +
                ":86:Line Information 2\r\n" +
                ":62F:C180225EUR3,00\r\n" +
                ":64:D180226EUR4,00\r\n" +
                ":65:C180227EUR5,00\r\n" +
                ":65:D180228EUR6,00\r\n" +
                ":86:Statement Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.AreEqual("Related Ref", statement.RelatedReference);
            }
        }

        [TestMethod]
        public void StatementParser_SingleCompleteStatement_ShouldParseField25()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/0987654321\r\n" +
                ":28C:12345/54321\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802220221CR1,00NRBTCustomer Ref 1//Bank Ref 1\r\n" +
                "Supplementary Details 1\r\n" +
                ":86:Line Information 1\r\n" +
                ":61:1802240223DF2,00NMSCCustomer Ref 2//Bank Ref 2\r\n" +
                "Supplementary Details 2\r\n" +
                ":86:Line Information 2\r\n" +
                ":62F:C180225EUR3,00\r\n" +
                ":64:D180226EUR4,00\r\n" +
                ":65:C180227EUR5,00\r\n" +
                ":65:D180228EUR6,00\r\n" +
                ":86:Statement Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.AreEqual("1234567890/0987654321", statement.AccountIdentification);
            }
        }

        [TestMethod]
        public void StatementParser_SingleCompleteStatement_ShouldParseField28CPart1()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/0987654321\r\n" +
                ":28C:12345/54321\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802220221CR1,00NRBTCustomer Ref 1//Bank Ref 1\r\n" +
                "Supplementary Details 1\r\n" +
                ":86:Line Information 1\r\n" +
                ":61:1802240223DF2,00NMSCCustomer Ref 2//Bank Ref 2\r\n" +
                "Supplementary Details 2\r\n" +
                ":86:Line Information 2\r\n" +
                ":62F:C180225EUR3,00\r\n" +
                ":64:D180226EUR4,00\r\n" +
                ":65:C180227EUR5,00\r\n" +
                ":65:D180228EUR6,00\r\n" +
                ":86:Statement Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.AreEqual(12345, statement.StatementNumber);
            }
        }

        [TestMethod]
        public void StatementParser_SingleCompleteStatement_ShouldParseField28CPart2()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/0987654321\r\n" +
                ":28C:12345/54321\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802220221CR1,00NRBTCustomer Ref 1//Bank Ref 1\r\n" +
                "Supplementary Details 1\r\n" +
                ":86:Line Information 1\r\n" +
                ":61:1802240223DF2,00NMSCCustomer Ref 2//Bank Ref 2\r\n" +
                "Supplementary Details 2\r\n" +
                ":86:Line Information 2\r\n" +
                ":62F:C180225EUR3,00\r\n" +
                ":64:D180226EUR4,00\r\n" +
                ":65:C180227EUR5,00\r\n" +
                ":65:D180228EUR6,00\r\n" +
                ":86:Statement Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.AreEqual(54321, statement.SequenceNumber);
            }
        }

        [TestMethod]
        public void StatementParser_SingleCompleteStatement_ShouldParseField60F()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/0987654321\r\n" +
                ":28C:12345/54321\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802220221CR1,00NRBTCustomer Ref 1//Bank Ref 1\r\n" +
                "Supplementary Details 1\r\n" +
                ":86:Line Information 1\r\n" +
                ":61:1802240223DF2,00NMSCCustomer Ref 2//Bank Ref 2\r\n" +
                "Supplementary Details 2\r\n" +
                ":86:Line Information 2\r\n" +
                ":62F:C180225EUR3,00\r\n" +
                ":64:D180226EUR4,00\r\n" +
                ":65:C180227EUR5,00\r\n" +
                ":65:D180228EUR6,00\r\n" +
                ":86:Statement Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.IsNotNull(statement.OpeningBalance);
            }
        }

        [TestMethod]
        public void StatementParser_SingleCompleteStatement_ShouldParseField60M()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/0987654321\r\n" +
                ":28C:12345/54321\r\n" +
                ":60M:C180220EUR0,00\r\n" +
                ":61:1802220221CR1,00NRBTCustomer Ref 1//Bank Ref 1\r\n" +
                "Supplementary Details 1\r\n" +
                ":86:Line Information 1\r\n" +
                ":61:1802240223DF2,00NMSCCustomer Ref 2//Bank Ref 2\r\n" +
                "Supplementary Details 2\r\n" +
                ":86:Line Information 2\r\n" +
                ":62F:C180225EUR3,00\r\n" +
                ":64:D180226EUR4,00\r\n" +
                ":65:C180227EUR5,00\r\n" +
                ":65:D180228EUR6,00\r\n" +
                ":86:Statement Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.IsNotNull(statement.OpeningBalance);
            }
        }

        [TestMethod]
        public void StatementParser_SingleCompleteStatement_ShouldParseField60FAsOpeningBalance()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/0987654321\r\n" +
                ":28C:12345/54321\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802220221CR1,00NRBTCustomer Ref 1//Bank Ref 1\r\n" +
                "Supplementary Details 1\r\n" +
                ":86:Line Information 1\r\n" +
                ":61:1802240223DF2,00NMSCCustomer Ref 2//Bank Ref 2\r\n" +
                "Supplementary Details 2\r\n" +
                ":86:Line Information 2\r\n" +
                ":62F:C180225EUR3,00\r\n" +
                ":64:D180226EUR4,00\r\n" +
                ":65:C180227EUR5,00\r\n" +
                ":65:D180228EUR6,00\r\n" +
                ":86:Statement Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.AreEqual(BalanceType.Opening, statement.OpeningBalance.Type);
            }
        }

        [TestMethod]
        public void StatementParser_SingleCompleteStatement_ShouldParseField60MAsIntermediateBalance()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/0987654321\r\n" +
                ":28C:12345/54321\r\n" +
                ":60M:C180220EUR0,00\r\n" +
                ":61:1802220221CR1,00NRBTCustomer Ref 1//Bank Ref 1\r\n" +
                "Supplementary Details 1\r\n" +
                ":86:Line Information 1\r\n" +
                ":61:1802240223DF2,00NMSCCustomer Ref 2//Bank Ref 2\r\n" +
                "Supplementary Details 2\r\n" +
                ":86:Line Information 2\r\n" +
                ":62F:C180225EUR3,00\r\n" +
                ":64:D180226EUR4,00\r\n" +
                ":65:C180227EUR5,00\r\n" +
                ":65:D180228EUR6,00\r\n" +
                ":86:Statement Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.AreEqual(BalanceType.Intermediate, statement.OpeningBalance.Type);
            }
        }

        [TestMethod]
        public void StatementParser_SingleCompleteStatement_ShouldParseField61()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/0987654321\r\n" +
                ":28C:12345/54321\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802220221CR1,00NRBTCustomer Ref 1//Bank Ref 1\r\n" +
                "Supplementary Details 1\r\n" +
                ":86:Line Information 1\r\n" +
                ":61:1802240223DF2,00NMSCCustomer Ref 2//Bank Ref 2\r\n" +
                "Supplementary Details 2\r\n" +
                ":86:Line Information 2\r\n" +
                ":62F:C180225EUR3,00\r\n" +
                ":64:D180226EUR4,00\r\n" +
                ":65:C180227EUR5,00\r\n" +
                ":65:D180228EUR6,00\r\n" +
                ":86:Statement Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.AreEqual(2, statement.Lines.Count);
            }
        }

        [TestMethod]
        public void StatementParser_SingleCompleteStatement_ShouldParseField61InOrder()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/0987654321\r\n" +
                ":28C:12345/54321\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802220221CR1,00NRBTCustomer Ref 1//Bank Ref 1\r\n" +
                "Supplementary Details 1\r\n" +
                ":86:Line Information 1\r\n" +
                ":61:1802240223DF2,00NMSCCustomer Ref 2//Bank Ref 2\r\n" +
                "Supplementary Details 2\r\n" +
                ":86:Line Information 2\r\n" +
                ":62F:C180225EUR3,00\r\n" +
                ":64:D180226EUR4,00\r\n" +
                ":65:C180227EUR5,00\r\n" +
                ":65:D180228EUR6,00\r\n" +
                ":86:Statement Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.AreEqual("Customer Ref 1", statement.Lines[0].CustomerReference);
                Assert.AreEqual("Customer Ref 2", statement.Lines[1].CustomerReference);
            }
        }

        [TestMethod]
        public void StatementParser_SingleCompleteStatement_ShouldParseField86InLine()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/0987654321\r\n" +
                ":28C:12345/54321\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802220221CR1,00NRBTCustomer Ref 1//Bank Ref 1\r\n" +
                "Supplementary Details 1\r\n" +
                ":86:Line Information 1\r\n" +
                ":61:1802240223DF2,00NMSCCustomer Ref 2//Bank Ref 2\r\n" +
                "Supplementary Details 2\r\n" +
                ":86:Line Information 2\r\n" +
                ":62F:C180225EUR3,00\r\n" +
                ":64:D180226EUR4,00\r\n" +
                ":65:C180227EUR5,00\r\n" +
                ":65:D180228EUR6,00\r\n" +
                ":86:Statement Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.AreEqual("Line Information 1", statement.Lines[0].InformationToOwner.UnstructuredData);
                Assert.AreEqual("Line Information 2", statement.Lines[1].InformationToOwner.UnstructuredData);
            }
        }

        [TestMethod]
        public void StatementParser_SingleCompleteStatement_ShouldParseField62F()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/0987654321\r\n" +
                ":28C:12345/54321\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802220221CR1,00NRBTCustomer Ref 1//Bank Ref 1\r\n" +
                "Supplementary Details 1\r\n" +
                ":86:Line Information 1\r\n" +
                ":61:1802240223DF2,00NMSCCustomer Ref 2//Bank Ref 2\r\n" +
                "Supplementary Details 2\r\n" +
                ":86:Line Information 2\r\n" +
                ":62F:C180225EUR3,00\r\n" +
                ":64:D180226EUR4,00\r\n" +
                ":65:C180227EUR5,00\r\n" +
                ":65:D180228EUR6,00\r\n" +
                ":86:Statement Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.IsNotNull(statement.ClosingBalance);
            }
        }

        [TestMethod]
        public void StatementParser_SingleCompleteStatement_ShouldParseField62M()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/0987654321\r\n" +
                ":28C:12345/54321\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802220221CR1,00NRBTCustomer Ref 1//Bank Ref 1\r\n" +
                "Supplementary Details 1\r\n" +
                ":86:Line Information 1\r\n" +
                ":61:1802240223DF2,00NMSCCustomer Ref 2//Bank Ref 2\r\n" +
                "Supplementary Details 2\r\n" +
                ":86:Line Information 2\r\n" +
                ":62M:C180225EUR3,00\r\n" +
                ":64:D180226EUR4,00\r\n" +
                ":65:C180227EUR5,00\r\n" +
                ":65:D180228EUR6,00\r\n" +
                ":86:Statement Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.IsNotNull(statement.ClosingBalance);
            }
        }

        [TestMethod]
        public void StatementParser_SingleCompleteStatement_ShouldParseField62FAsOpeningBalance()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/0987654321\r\n" +
                ":28C:12345/54321\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802220221CR1,00NRBTCustomer Ref 1//Bank Ref 1\r\n" +
                "Supplementary Details 1\r\n" +
                ":86:Line Information 1\r\n" +
                ":61:1802240223DF2,00NMSCCustomer Ref 2//Bank Ref 2\r\n" +
                "Supplementary Details 2\r\n" +
                ":86:Line Information 2\r\n" +
                ":62F:C180225EUR3,00\r\n" +
                ":64:D180226EUR4,00\r\n" +
                ":65:C180227EUR5,00\r\n" +
                ":65:D180228EUR6,00\r\n" +
                ":86:Statement Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.AreEqual(BalanceType.Closing, statement.ClosingBalance.Type);
            }
        }

        [TestMethod]
        public void StatementParser_SingleCompleteStatement_ShouldParseField62MAsIntermediateBalance()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/0987654321\r\n" +
                ":28C:12345/54321\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802220221CR1,00NRBTCustomer Ref 1//Bank Ref 1\r\n" +
                "Supplementary Details 1\r\n" +
                ":86:Line Information 1\r\n" +
                ":61:1802240223DF2,00NMSCCustomer Ref 2//Bank Ref 2\r\n" +
                "Supplementary Details 2\r\n" +
                ":86:Line Information 2\r\n" +
                ":62M:C180225EUR3,00\r\n" +
                ":64:D180226EUR4,00\r\n" +
                ":65:C180227EUR5,00\r\n" +
                ":65:D180228EUR6,00\r\n" +
                ":86:Statement Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.AreEqual(BalanceType.Intermediate, statement.ClosingBalance.Type);
            }
        }

        [TestMethod]
        public void StatementParser_SingleCompleteStatement_ShouldParseField64()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/0987654321\r\n" +
                ":28C:12345/54321\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802220221CR1,00NRBTCustomer Ref 1//Bank Ref 1\r\n" +
                "Supplementary Details 1\r\n" +
                ":86:Line Information 1\r\n" +
                ":61:1802240223DF2,00NMSCCustomer Ref 2//Bank Ref 2\r\n" +
                "Supplementary Details 2\r\n" +
                ":86:Line Information 2\r\n" +
                ":62F:C180225EUR3,00\r\n" +
                ":64:D180226EUR4,00\r\n" +
                ":65:C180227EUR5,00\r\n" +
                ":65:D180228EUR6,00\r\n" +
                ":86:Statement Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.IsNotNull(statement.ClosingAvailableBalance);
            }
        }

        [TestMethod]
        public void StatementParser_SingleCompleteStatement_ShouldParseField65()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/0987654321\r\n" +
                ":28C:12345/54321\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802220221CR1,00NRBTCustomer Ref 1//Bank Ref 1\r\n" +
                "Supplementary Details 1\r\n" +
                ":86:Line Information 1\r\n" +
                ":61:1802240223DF2,00NMSCCustomer Ref 2//Bank Ref 2\r\n" +
                "Supplementary Details 2\r\n" +
                ":86:Line Information 2\r\n" +
                ":62F:C180225EUR3,00\r\n" +
                ":64:D180226EUR4,00\r\n" +
                ":65:C180227EUR5,00\r\n" +
                ":65:D180228EUR6,00\r\n" +
                ":86:Statement Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.AreEqual(2, statement.ForwardAvailableBalances.Count);
            }
        }

        [TestMethod]
        public void StatementParser_SingleCompleteStatement_ShouldParseField65InOrder()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/0987654321\r\n" +
                ":28C:12345/54321\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802220221CR1,00NRBTCustomer Ref 1//Bank Ref 1\r\n" +
                "Supplementary Details 1\r\n" +
                ":86:Line Information 1\r\n" +
                ":61:1802240223DF2,00NMSCCustomer Ref 2//Bank Ref 2\r\n" +
                "Supplementary Details 2\r\n" +
                ":86:Line Information 2\r\n" +
                ":62F:C180225EUR3,00\r\n" +
                ":64:D180226EUR4,00\r\n" +
                ":65:C180227EUR5,00\r\n" +
                ":65:D180228EUR6,00\r\n" +
                ":86:Statement Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.AreEqual(5m, statement.ForwardAvailableBalances[0].Amount);
                Assert.AreEqual(6m, statement.ForwardAvailableBalances[1].Amount);
            }
        }

        [TestMethod]
        public void StatementParser_SingleCompleteStatement_ShouldParseField86AtEndOfStatement()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/0987654321\r\n" +
                ":28C:12345/54321\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802220221CR1,00NRBTCustomer Ref 1//Bank Ref 1\r\n" +
                "Supplementary Details 1\r\n" +
                ":86:Line Information 1\r\n" +
                ":61:1802240223DF2,00NMSCCustomer Ref 2//Bank Ref 2\r\n" +
                "Supplementary Details 2\r\n" +
                ":86:Line Information 2\r\n" +
                ":62F:C180225EUR3,00\r\n" +
                ":64:D180226EUR4,00\r\n" +
                ":65:C180227EUR5,00\r\n" +
                ":65:D180228EUR6,00\r\n" +
                ":86:Statement Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.AreEqual("Statement Information", statement.InformationToOwner.UnstructuredData);
            }
        }

        [TestMethod]
        public void StatementParser_MultipleStatements_ShouldParseOneStatementAfterTheOther()
        {
            var text = "\r\n" +
                ":20:Statement 1\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/0987654321\r\n" +
                ":28C:12345/54321\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802220221CR1,00NRBTCustomer Ref 1//Bank Ref 1\r\n" +
                "Supplementary Details 1\r\n" +
                ":86:Line Information 1\r\n" +
                ":61:1802240223DF2,00NMSCCustomer Ref 2//Bank Ref 2\r\n" +
                "Supplementary Details 2\r\n" +
                ":86:Line Information 2\r\n" +
                ":62F:C180225EUR3,00\r\n" +
                ":64:D180226EUR4,00\r\n" +
                ":65:C180227EUR5,00\r\n" +
                ":65:D180228EUR6,00\r\n" +
                ":86:Statement Information\r\n" +
                "-";

            text = text +
                   text.Replace("Statement 1", "Statement 2") +
                   text.Replace("Statement 1", "Statement 3");

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);

                var statement1 = parser.ReadStatement();
                var statement2 = parser.ReadStatement();
                var statement3 = parser.ReadStatement();

                Assert.AreEqual("Statement 1", statement1.TransactionReferenceNumber);
                Assert.AreEqual("Statement 2", statement2.TransactionReferenceNumber);
                Assert.AreEqual("Statement 3", statement3.TransactionReferenceNumber);
            }
        }


        #endregion

        #region Whitespace and specific data types

        [TestMethod]
        public void StatementParser_Field20OnlyWhitespace_ShouldParseWhitespace()
        {
            var text = "\r\n" +
                ":20:  \t\t  \r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/0987654321\r\n" +
                ":28C:12345/54321\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802220221CR1,00NRBTCustomer Ref 1//Bank Ref 1\r\n" +
                "Supplementary Details 1\r\n" +
                ":86:Line Information 1\r\n" +
                ":61:1802240223DF2,00NMSCCustomer Ref 2//Bank Ref 2\r\n" +
                "Supplementary Details 2\r\n" +
                ":86:Line Information 2\r\n" +
                ":62F:C180225EUR3,00\r\n" +
                ":64:D180226EUR4,00\r\n" +
                ":65:C180227EUR5,00\r\n" +
                ":65:D180228EUR6,00\r\n" +
                ":86:Statement Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.AreEqual("  \t\t  ", statement.TransactionReferenceNumber);
            }
        }

        [TestMethod]
        public void StatementParser_Field21OnlyWhitespace_ShouldParseWhitespace()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:  \t\t  \r\n" +
                ":25:1234567890/0987654321\r\n" +
                ":28C:12345/54321\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802220221CR1,00NRBTCustomer Ref 1//Bank Ref 1\r\n" +
                "Supplementary Details 1\r\n" +
                ":86:Line Information 1\r\n" +
                ":61:1802240223DF2,00NMSCCustomer Ref 2//Bank Ref 2\r\n" +
                "Supplementary Details 2\r\n" +
                ":86:Line Information 2\r\n" +
                ":62F:C180225EUR3,00\r\n" +
                ":64:D180226EUR4,00\r\n" +
                ":65:C180227EUR5,00\r\n" +
                ":65:D180228EUR6,00\r\n" +
                ":86:Statement Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.AreEqual("  \t\t  ", statement.RelatedReference);
            }
        }

        [TestMethod]
        public void StatementParser_Field25OnlyWhitespace_ShouldParseWhitespace()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:  \t\t  \r\n" +
                ":28C:12345/54321\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802220221CR1,00NRBTCustomer Ref 1//Bank Ref 1\r\n" +
                "Supplementary Details 1\r\n" +
                ":86:Line Information 1\r\n" +
                ":61:1802240223DF2,00NMSCCustomer Ref 2//Bank Ref 2\r\n" +
                "Supplementary Details 2\r\n" +
                ":86:Line Information 2\r\n" +
                ":62F:C180225EUR3,00\r\n" +
                ":64:D180226EUR4,00\r\n" +
                ":65:C180227EUR5,00\r\n" +
                ":65:D180228EUR6,00\r\n" +
                ":86:Statement Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.AreEqual("  \t\t  ", statement.AccountIdentification);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void StatementParser_Field28CPart1OnlyWhitespace_ShouldThrowFormatException()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/0987654321\r\n" +
                ":28C:  \t  /54321\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802220221CR1,00NRBTCustomer Ref 1//Bank Ref 1\r\n" +
                "Supplementary Details 1\r\n" +
                ":86:Line Information 1\r\n" +
                ":61:1802240223DF2,00NMSCCustomer Ref 2//Bank Ref 2\r\n" +
                "Supplementary Details 2\r\n" +
                ":86:Line Information 2\r\n" +
                ":62F:C180225EUR3,00\r\n" +
                ":64:D180226EUR4,00\r\n" +
                ":65:C180227EUR5,00\r\n" +
                ":65:D180228EUR6,00\r\n" +
                ":86:Statement Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void StatementParser_Field28CPart1NonNumeric_ShouldThrowFormatException()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/0987654321\r\n" +
                ":28C:12cd5/54321\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802220221CR1,00NRBTCustomer Ref 1//Bank Ref 1\r\n" +
                "Supplementary Details 1\r\n" +
                ":86:Line Information 1\r\n" +
                ":61:1802240223DF2,00NMSCCustomer Ref 2//Bank Ref 2\r\n" +
                "Supplementary Details 2\r\n" +
                ":86:Line Information 2\r\n" +
                ":62F:C180225EUR3,00\r\n" +
                ":64:D180226EUR4,00\r\n" +
                ":65:C180227EUR5,00\r\n" +
                ":65:D180228EUR6,00\r\n" +
                ":86:Statement Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void StatementParser_Field28CPart1DecimalSeparator_ShouldThrowFormatException()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/0987654321\r\n" +
                ":28C:12.45/54321\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802220221CR1,00NRBTCustomer Ref 1//Bank Ref 1\r\n" +
                "Supplementary Details 1\r\n" +
                ":86:Line Information 1\r\n" +
                ":61:1802240223DF2,00NMSCCustomer Ref 2//Bank Ref 2\r\n" +
                "Supplementary Details 2\r\n" +
                ":86:Line Information 2\r\n" +
                ":62F:C180225EUR3,00\r\n" +
                ":64:D180226EUR4,00\r\n" +
                ":65:C180227EUR5,00\r\n" +
                ":65:D180228EUR6,00\r\n" +
                ":86:Statement Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void StatementParser_Field28CPart2OnlyWhitespace_ShouldThrowFormatException()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/0987654321\r\n" +
                ":28C:12345/  \t  \r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802220221CR1,00NRBTCustomer Ref 1//Bank Ref 1\r\n" +
                "Supplementary Details 1\r\n" +
                ":86:Line Information 1\r\n" +
                ":61:1802240223DF2,00NMSCCustomer Ref 2//Bank Ref 2\r\n" +
                "Supplementary Details 2\r\n" +
                ":86:Line Information 2\r\n" +
                ":62F:C180225EUR3,00\r\n" +
                ":64:D180226EUR4,00\r\n" +
                ":65:C180227EUR5,00\r\n" +
                ":65:D180228EUR6,00\r\n" +
                ":86:Statement Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void StatementParser_Field28CPart2NonNumeric_ShouldThrowFormatException()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/0987654321\r\n" +
                ":28C:12345/54cb1\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802220221CR1,00NRBTCustomer Ref 1//Bank Ref 1\r\n" +
                "Supplementary Details 1\r\n" +
                ":86:Line Information 1\r\n" +
                ":61:1802240223DF2,00NMSCCustomer Ref 2//Bank Ref 2\r\n" +
                "Supplementary Details 2\r\n" +
                ":86:Line Information 2\r\n" +
                ":62F:C180225EUR3,00\r\n" +
                ":64:D180226EUR4,00\r\n" +
                ":65:C180227EUR5,00\r\n" +
                ":65:D180228EUR6,00\r\n" +
                ":86:Statement Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void StatementParser_Field28CPart2DecimalSeparator_ShouldThrowFormatException()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/0987654321\r\n" +
                ":28C:12345/54.21\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802220221CR1,00NRBTCustomer Ref 1//Bank Ref 1\r\n" +
                "Supplementary Details 1\r\n" +
                ":86:Line Information 1\r\n" +
                ":61:1802240223DF2,00NMSCCustomer Ref 2//Bank Ref 2\r\n" +
                "Supplementary Details 2\r\n" +
                ":86:Line Information 2\r\n" +
                ":62F:C180225EUR3,00\r\n" +
                ":64:D180226EUR4,00\r\n" +
                ":65:C180227EUR5,00\r\n" +
                ":65:D180228EUR6,00\r\n" +
                ":86:Statement Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();
            }
        }

        [TestMethod]
        public void StatementParser_Field86InLineOnlyWhitespace_ShouldParseWhitespace()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/0987654321\r\n" +
                ":28C:12345/54321\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802220221CR1,00NRBTCustomer Ref 1//Bank Ref 1\r\n" +
                "Supplementary Details 1\r\n" +
                ":86:Line Information 1\r\n" +
                ":61:1802240223DF2,00NMSCCustomer Ref 2//Bank Ref 2\r\n" +
                "Supplementary Details 2\r\n" +
                ":86:Line Information 2\r\n" +
                ":62F:C180225EUR3,00\r\n" +
                ":64:D180226EUR4,00\r\n" +
                ":65:C180227EUR5,00\r\n" +
                ":65:D180228EUR6,00\r\n" +
                ":86:  \t\t  \r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.AreEqual("  \t\t  ", statement.InformationToOwner.UnstructuredData);
            }
        }

        [TestMethod]
        public void StatementParser_EndOfStatementAfterField62a_ShouldParse()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/0987654321\r\n" +
                ":28C:12345/54321\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802220221CR1,00NRBTCustomer Ref 1//Bank Ref 1\r\n" +
                "Supplementary Details 1\r\n" +
                ":86:Line Information 1\r\n" +
                ":61:1802240223DF2,00NMSCCustomer Ref 2//Bank Ref 2\r\n" +
                "Supplementary Details 2\r\n" +
                ":86:Line Information 2\r\n" +
                ":62F:C180225EUR3,00\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.IsNotNull(statement);
            }
        }

        [TestMethod]
        public void StatementParser_EndOfStatementAfterField64_ShouldParse()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/0987654321\r\n" +
                ":28C:12345/54321\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802220221CR1,00NRBTCustomer Ref 1//Bank Ref 1\r\n" +
                "Supplementary Details 1\r\n" +
                ":86:Line Information 1\r\n" +
                ":61:1802240223DF2,00NMSCCustomer Ref 2//Bank Ref 2\r\n" +
                "Supplementary Details 2\r\n" +
                ":86:Line Information 2\r\n" +
                ":62F:C180225EUR3,00\r\n" +
                ":64:D180226EUR4,00\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.IsNotNull(statement);
            }
        }

        [TestMethod]
        public void StatementParser_EndOfStatementAfterField65_ShouldParse()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/0987654321\r\n" +
                ":28C:12345/54321\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802220221CR1,00NRBTCustomer Ref 1//Bank Ref 1\r\n" +
                "Supplementary Details 1\r\n" +
                ":86:Line Information 1\r\n" +
                ":61:1802240223DF2,00NMSCCustomer Ref 2//Bank Ref 2\r\n" +
                "Supplementary Details 2\r\n" +
                ":86:Line Information 2\r\n" +
                ":62F:C180225EUR3,00\r\n" +
                ":64:D180226EUR4,00\r\n" +
                ":65:C180227EUR5,00\r\n" +
                ":65:D180228EUR6,00\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.IsNotNull(statement);
            }
        }

        [TestMethod]
        public void StatementParser_EndOfStatementAfterField86_ShouldParse()
        {
            var text = "\r\n" +
                ":20:Transaction Ref\r\n" +
                ":21:Related Ref\r\n" +
                ":25:1234567890/0987654321\r\n" +
                ":28C:12345/54321\r\n" +
                ":60F:C180220EUR0,00\r\n" +
                ":61:1802220221CR1,00NRBTCustomer Ref 1//Bank Ref 1\r\n" +
                "Supplementary Details 1\r\n" +
                ":86:Line Information 1\r\n" +
                ":61:1802240223DF2,00NMSCCustomer Ref 2//Bank Ref 2\r\n" +
                "Supplementary Details 2\r\n" +
                ":86:Line Information 2\r\n" +
                ":62F:C180225EUR3,00\r\n" +
                ":64:D180226EUR4,00\r\n" +
                ":65:C180227EUR5,00\r\n" +
                ":65:D180228EUR6,00\r\n" +
                ":86:Statement Information\r\n" +
                "-";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var parser = new StatementParser(reader);
                var statement = parser.ReadStatement();

                Assert.IsNotNull(statement);
            }
        }

        #endregion
    }
}
