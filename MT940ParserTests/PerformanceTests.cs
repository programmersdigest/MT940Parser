using Microsoft.VisualStudio.TestTools.UnitTesting;
using programmersdigest.MT940Parser;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace MT940ParserTests
{
    [TestClass]
    public class PerformanceTests
    {
        [TestMethod]
        public void Parser_Parse100000Statements()
        {
            Assert.Inconclusive("Please uncomment this line to run the performance test.");

            var stopwatch = new Stopwatch();
            var text = "\r\n" +
                ":20:1234567890123456\r\n" +
                ":21:1234567890123456\r\n" +
                ":25:1234567890123456789/123456789012345\r\n" +
                ":28C:12345/12345\r\n" +
                ":60F:C180220EUR123456789,12345\r\n" +
                ":61:1802200220RDR123456789,12345NMSC1234567890123456//1234567890123456\r\n" +
                "1234567890123456789012345678901234\r\n" +
                ":86:105" +
                "?00abcdefghijklmnopqrstuvwxyza" +
                "?101234567890" +
                "?20EREF+abcdefghijklmnopqrstuv" +
                "?21KREF+abcdefghijklmnopqrstuv" +
                "?22MREF+abcdefghijklmnopqrstuv" +
                "?23CRED+abcdefghijklmnopqrstuv" +
                "?24DEBT+abcdefghijklmnopqrstuv" +
                "?25COAM+abcdefghijklmnopqrstuv" +
                "?26OAMT+abcdefghijklmnopqrstuv" +
                "?27SVWZ+abcdefghijklmnopqrstuv" +
                "?28ABWA+abcdefghijklmnopqrstuv" +
                "?29ABWE+abcdefghijklmnopqrstuv" +
                "?30123456789012" +
                "?311234567890123456789012345678901234" +
                "?32abcdefghijklmnopqrstuvwxyza" +
                "?33abcdefghijklmnopqrstuvwxyza" +
                "?34123" +
                "?60abcdefghijklmnopqrstuvwxyza" +
                "?61abcdefghijklmnopqrstuvwxyza" +
                "?62abcdefghijklmnopqrstuvwxyza" +
                "?63abcdefghijklmnopqrstuvwxyza\r\n" +
                ":62F:C180220EUR123456789,12345\r\n" +
                ":64:C180220EUR123456789,12345\r\n" +
                ":65:C180220EUR123456789,12345\r\n" +
                ":86:999456789012345678901234567890123456789012345678901234567890123\r\n" +
                "123456789012345678901234567890123456789012345678901234567890123\r\n" +
                "123456789012345678901234567890123456789012345678901234567890123\r\n" +
                "123456789012345678901234567890123456789012345678901234567890123\r\n" +
                "123456789012345678901234567890123456789012345678901234567890123\r\n" +
                "123456789012345678901234567890123456789012345678901234567890123\r\n" +
                "-";

            var count = 0;

            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream, Encoding.ASCII, 1024, true))
                {
                    for (int i = 0; i < 100000; i++)
                    {
                        writer.Write(text);
                    }
                }
                stream.Seek(0, SeekOrigin.Begin);

                var parser = new Parser(stream);

                stopwatch.Start();
                foreach (var statement in parser.Parse())
                {
                    count++;
                }
                stopwatch.Stop();
            }

            Assert.Inconclusive($"Parsed {count} statements in: {stopwatch.Elapsed}");
        }
    }
}
