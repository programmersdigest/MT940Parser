using Microsoft.VisualStudio.TestTools.UnitTesting;
using programmersdigest.MT940Parser;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace MT940ParserTests
{
    [TestClass]
    public class ParserTests
    {
        #region Input checks

        [TestMethod]
        public void Parser_InputIsEmpty_ShouldReturnZeroStatements()
        {
            var text = "";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var parser = new Parser(stream))
            {
                var statements = parser.Parse();

                Assert.AreEqual(0, statements.Count());
            }
        }

        [TestMethod]
        public void Parser_InputIsNonsensical_ShouldReturnZeroStatements()
        {
            var text = "32u1mbß61zßv";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var parser = new Parser(stream))
            {
                var statements = parser.Parse();

                Assert.AreEqual(0, statements.Count());
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Parser_StreamIsNull_ShouldThrowArgumentNullException()
        {
            Stream stream = null;

            using (var parser = new Parser(stream))
            {
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Parser_PathIsNull_ShouldThrowArgumentNullException()
        {
            string path = null;

            using (var parser = new Parser(path))
            {
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Parser_PathIsEmpty_ShouldThrowArgumentNullException()
        {
            var path = "";

            using (var parser = new Parser(path))
            {
            }
        }

        #endregion

        #region Positive results

        [TestMethod]
        public void Parser_SingleStatement_ShouldParse()
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
                "-\r\n";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var parser = new Parser(stream))
            {
                var statements = parser.Parse();

                Assert.AreEqual(1, statements.Count());
            }
        }

        [TestMethod]
        public void Parser_MultipleStatements_ShouldParse()
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
                "-\r\n";
            text = string.Concat(text, text, text, text, text);     // Generate five statement.

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var parser = new Parser(stream))
            {
                var statements = parser.Parse();

                Assert.AreEqual(5, statements.Count());
            }
        }

        #endregion
    }
}
