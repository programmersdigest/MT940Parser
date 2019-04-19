using Microsoft.VisualStudio.TestTools.UnitTesting;
using programmersdigest.MT940Parser.Parsing;
using System;
using System.IO;
using System.Text;

namespace MT940ParserTests
{
    [TestClass]
    public class StreamReaderExtensionsTests
    {
        #region StreamReader_Find

        [TestMethod]
        public void StreamReader_Find_NeedleExists_ShouldMoveBehindNeedle()
        {
            var text = "12Needle3456789";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                reader.Find("Needle");

                var next = (char)reader.Peek();
                Assert.AreEqual('3', next);
            }
        }

        [TestMethod]
        public void StreamReader_Find_NeedleAtStart_ShouldMoveBehindNeedle()
        {
            var text = "Needle123456789";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                reader.Find("Needle");

                var next = (char)reader.Peek();
                Assert.AreEqual('1', next);
            }
        }

        [TestMethod]
        public void StreamReader_Find_NeedleAtEnd_ShouldMoveBehindNeedle()
        {
            var text = "123456789Needle";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                reader.Find("Needle");

                Assert.IsTrue(reader.EndOfStream);
            }
        }

        [TestMethod]
        public void StreamReader_Find_NeedleMultipleTimes_ShouldFindOneAfterTheOther()
        {
            var text = "Needle123Needle45678Needle9";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                // First
                reader.Find("Needle");

                var next = (char)reader.Peek();
                Assert.AreEqual('1', next);

                // Second
                reader.Find("Needle");

                next = (char)reader.Peek();
                Assert.AreEqual('4', next);

                // Third
                reader.Find("Needle");

                next = (char)reader.Peek();
                Assert.AreEqual('9', next);
            }
        }

        [TestMethod]
        public void StreamReader_Find_NeedleMultipleTimesInSuccession_ShouldFindOneAfterTheOther()
        {
            var text = "123NeedleNeedle456789";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                // First
                reader.Find("Needle");

                var next = (char)reader.Peek();
                Assert.AreEqual('N', next);

                // Second
                reader.Find("Needle");

                next = (char)reader.Peek();
                Assert.AreEqual('4', next);
            }
        }

        [TestMethod]
        public void StreamReader_Find_NeedleDoesNotExist_ShouldGoToEndOfStream()
        {
            var text = "123456789";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                reader.Find("NoNeedle");

                Assert.IsTrue(reader.EndOfStream);
            }
        }

        [TestMethod]
        public void StreamReader_Find_PartialMatchBeforeNeedle_ShouldIgnorePartialMatch()
        {
            var text = "12NeeNeedle3456789";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                reader.Find("Needle");

                var next = (char)reader.Peek();
                Assert.AreEqual('3', next);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StreamReader_Find_ReaderIsNull_ShouldThrowException()
        {
            StreamReaderExtensions.Find(null, "Test");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void StreamReader_Find_NeedleIsEmpty_ShouldThrowException()
        {
            var text = "123456789";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                reader.Find("");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StreamReader_Find_NeedleIsNull_ShouldThrowException()
        {
            var text = "123456789";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                reader.Find(null);
            }
        }

        #endregion

        #region StreamReader_ReadTo

        [TestMethod]
        public void StreamReader_ReadTo_SingleNeedle_NeedleExists_ShouldReturnFirstPartExcludingNeedle()
        {
            var text = "12Needle3456789";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var result = reader.ReadTo(out var needle, "Needle");

                Assert.AreEqual("12", result);
            }
        }

        [TestMethod]
        public void StreamReader_ReadTo_SingleNeedle_NeedleExists_ShouldMoveBehindNeedle()
        {
            var text = "12Needle3456789";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var result = reader.ReadTo(out var needle, "Needle");

                var next = (char)reader.Peek();
                Assert.AreEqual('3', next);
            }
        }

        [TestMethod]
        public void StreamReader_ReadTo_SingleNeedle_NeedleExists_ShouldOutputFoundNeedle()
        {
            var text = "12Needle3456789";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var result = reader.ReadTo(out var needle, "Needle");

                Assert.AreEqual("Needle", needle);
            }
        }

        [TestMethod]
        public void StreamReader_ReadTo_SingleNeedle_NeedleAtStart_ShouldReturnEmptyString()
        {
            var text = "Needle123456789";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var result = reader.ReadTo(out var needle, "Needle");

                Assert.AreEqual("", result);
            }
        }

        [TestMethod]
        public void StreamReader_ReadTo_SingleNeedle_NeedleAtStart_ShouldMoveBehindNeedle()
        {
            var text = "Needle123456789";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var result = reader.ReadTo(out var needle, "Needle");

                var next = (char)reader.Peek();
                Assert.AreEqual('1', next);
            }
        }

        [TestMethod]
        public void StreamReader_ReadTo_SingleNeedle_NeedleAtStart_ShouldOutputFoundNeedle()
        {
            var text = "Needle123456789";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var result = reader.ReadTo(out var needle, "Needle");

                Assert.AreEqual("Needle", needle);
            }
        }

        [TestMethod]
        public void StreamReader_ReadTo_SingleNeedle_NeedleAtEnd_ShouldReturnFirstPartExcludingNeedle()
        {
            var text = "123456789Needle";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var result = reader.ReadTo(out var needle, "Needle");

                Assert.AreEqual("123456789", result);
            }
        }

        [TestMethod]
        public void StreamReader_ReadTo_SingleNeedle_NeedleAtEnd_ShouldMoveBehindNeedle()
        {
            var text = "123456789Needle";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var result = reader.ReadTo(out var needle, "Needle");

                Assert.IsTrue(reader.EndOfStream);
            }
        }

        [TestMethod]
        public void StreamReader_ReadTo_SingleNeedle_NeedleAtEnd_ShouldOutputFoundNeedle()
        {
            var text = "123456789Needle";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var result = reader.ReadTo(out var needle, "Needle");

                Assert.AreEqual("Needle", needle);
            }
        }

        [TestMethod]
        public void StreamReader_ReadTo_SingleNeedleMultipleTimes_ShouldReadOneAfterTheOther()
        {
            var text = "Needle123Needle45678Needle9";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                // First
                var result = reader.ReadTo(out var needle, "Needle");
                Assert.AreEqual("", result);

                // Second
                result = reader.ReadTo(out needle, "Needle");
                Assert.AreEqual("123", result);

                // Third
                result = reader.ReadTo(out needle, "Needle");
                Assert.AreEqual("45678", result);
            }
        }

        [TestMethod]
        public void StreamReader_ReadTo_SingleNeedleMultipleTimesInSuccession_ShouldFindOneAfterTheOther()
        {
            var text = "123NeedleNeedle456789";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                // First
                var result = reader.ReadTo(out var needle, "Needle");
                Assert.AreEqual("123", result);

                // Second
                result = reader.ReadTo(out needle, "Needle");
                Assert.AreEqual("", result);
            }
        }

        [TestMethod]
        public void StreamReader_ReadTo_SingleNeedleDoesNotExist_ShouldReturnWholeString()
        {
            var text = "123456789";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var result = reader.ReadTo(out var needle, "NoNeedle");

                Assert.AreEqual("123456789", result);
            }
        }

        [TestMethod]
        public void StreamReader_ReadTo_SingleNeedleDoesNotExist_ShouldOutputNeedleNull()
        {
            var text = "123456789";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var result = reader.ReadTo(out var needle, "NoNeedle");

                Assert.AreEqual(null, needle);
            }
        }

        [TestMethod]
        public void StreamReader_ReadTo_SingleNeedleDoesNotExist_ShouldGoToEndOfStream()
        {
            var text = "123456789";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var result = reader.ReadTo(out var needle, "NoNeedle");

                Assert.IsTrue(reader.EndOfStream);
            }
        }

        [TestMethod]
        public void StreamReader_ReadTo_SingleNeedle_PartialMatchBeforeNeedle_ShouldReturnFirstPartExcludingNeedle()
        {
            var text = "12NeeNeedle3456789";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var result = reader.ReadTo(out var needle, "Needle");

                Assert.AreEqual("12Nee", result);
            }
        }

        [TestMethod]
        public void StreamReader_ReadTo_SingleNeedle_PartialMatchBeforeNeedle_ShouldMoveBehindNeedle()
        {
            var text = "12NeeNeedle3456789";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var result = reader.ReadTo(out var needle, "Needle");

                var next = (char)reader.Peek();
                Assert.AreEqual('3', next);
            }
        }

        [TestMethod]
        public void StreamReader_ReadTo_SingleNeedle_PartialMatchBeforeNeedle_ShouldOutputFoundNeedle()
        {
            var text = "12NeeNeedle3456789";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var result = reader.ReadTo(out var needle, "Needle");

                Assert.AreEqual("Needle", needle);
            }
        }

        [TestMethod]
        public void StreamReader_ReadTo_MultipleNeedles_OneNeedleExists_ShouldReturnFirstPartExcludingNeedle()
        {
            var text = "12Needle3456789";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var result = reader.ReadTo(out var needle, "Needle", "Yarn");

                Assert.AreEqual("12", result);
            }
        }

        [TestMethod]
        public void StreamReader_ReadTo_MultipleNeedles_OneNeedleExists_ShouldMoveBehindNeedle()
        {
            var text = "12Needle3456789";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var result = reader.ReadTo(out var needle, "Needle", "Yarn");

                var next = (char)reader.Peek();
                Assert.AreEqual('3', next);
            }
        }

        [TestMethod]
        public void StreamReader_ReadTo_MultipleNeedles_OneNeedleExists_ShouldOutputFoundNeedle()
        {
            var text = "12Needle3456789";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var result = reader.ReadTo(out var needle, "Needle", "Yarn");

                Assert.AreEqual("Needle", needle);
            }
        }

        [TestMethod]
        public void StreamReader_ReadTo_MultipleNeedlesMultipleTimes_ShouldReadOneAfterTheOther()
        {
            var text = "Needle123Yarn45678Needle9";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                // First
                var result = reader.ReadTo(out var needle, "Needle", "Yarn");
                Assert.AreEqual("", result);

                // Second
                result = reader.ReadTo(out needle, "Needle", "Yarn");
                Assert.AreEqual("123", result);

                // Third
                result = reader.ReadTo(out needle, "Needle", "Yarn");
                Assert.AreEqual("45678", result);
            }
        }

        [TestMethod]
        public void StreamReader_ReadTo_MultipleNeedlesMultipleTimesInSuccession_ShouldFindOneAfterTheOther()
        {
            var text = "123NeedleYarnYarnNeedle456789";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                // First
                var result = reader.ReadTo(out var needle, "Needle", "Yarn");
                Assert.AreEqual("123", result);

                // Second
                result = reader.ReadTo(out needle, "Needle", "Yarn");
                Assert.AreEqual("", result);

                // Third
                result = reader.ReadTo(out needle, "Needle", "Yarn");
                Assert.AreEqual("", result);

                // Fourth
                result = reader.ReadTo(out needle, "Needle", "Yarn");
                Assert.AreEqual("", result);
            }
        }

        [TestMethod]
        public void StreamReader_ReadTo_MultipleNeedlesNoneExist_ShouldReturnWholeString()
        {
            var text = "123456789";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var result = reader.ReadTo(out var needle, "NoNeedle", "NoYarn");

                Assert.AreEqual("123456789", result);
            }
        }

        [TestMethod]
        public void StreamReader_ReadTo_MultipleNeedlesNoneExist_ShouldOutputNeedleNull()
        {
            var text = "123456789";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var result = reader.ReadTo(out var needle, "NoNeedle", "NoYarn");

                Assert.AreEqual(null, needle);
            }
        }

        [TestMethod]
        public void StreamReader_ReadTo_MultipleNeedlesNoneExist_ShouldGoToEndOfStream()
        {
            var text = "123456789";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var result = reader.ReadTo(out var needle, "NoNeedle", "NoYarn");

                Assert.IsTrue(reader.EndOfStream);
            }
        }

        [TestMethod]
        public void StreamReader_ReadTo_MultipleNeedles_NeedlesExist_OrderShouldNotMatter()
        {
            var text = "12Yarn3456Needle789";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                // First
                var result = reader.ReadTo(out var needle, "Needle", "Yarn");
                Assert.AreEqual("12", result);

                // Second
                result = reader.ReadTo(out needle, "Needle", "Yarn");
                Assert.AreEqual("3456", result);
            }

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                // First Reversed
                var result = reader.ReadTo(out var needle, "Yarn", "Needle");
                Assert.AreEqual("12", result);

                // Second Reversed
                result = reader.ReadTo(out needle, "Yarn", "Needle");
                Assert.AreEqual("3456", result);
            }
        }

        [TestMethod]
        public void StreamReader_ReadTo_FiveNeedlesMultipleTimes_ShouldFindOneAfterTheOther()
        {
            var text = "12Mickey345Donald6Goofy789Dagobert";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                // First
                var result = reader.ReadTo(out var needle, "Mickey", "Donald", "Oswald", "Goofy", "Dagobert");
                Assert.AreEqual("12", result);
                Assert.AreEqual("Mickey", needle);

                // Second
                result = reader.ReadTo(out needle, "Mickey", "Donald", "Oswald", "Goofy", "Dagobert");
                Assert.AreEqual("345", result);
                Assert.AreEqual("Donald", needle);

                // Third
                result = reader.ReadTo(out needle, "Mickey", "Donald", "Oswald", "Goofy", "Dagobert");
                Assert.AreEqual("6", result);
                Assert.AreEqual("Goofy", needle);

                // Fourth
                result = reader.ReadTo(out needle, "Mickey", "Donald", "Oswald", "Goofy", "Dagobert");
                Assert.AreEqual("789", result);
                Assert.AreEqual("Dagobert", needle);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StreamReader_ReadTo_ReaderIsNull_ShouldThrowException()
        {
            var result = StreamReaderExtensions.ReadTo(null, out var needle, "Test");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void StreamReader_ReadTo_NeedlesIsEmpty_ShouldThrowException()
        {
            var text = "123456789";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var result = reader.ReadTo(out var needle);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StreamReader_ReadTo_NeedlesIsNull_ShouldThrowException()
        {
            var text = "123456789";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var result = reader.ReadTo(out var needle, null);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void StreamReader_ReadTo_OneNeedleIsNull_ShouldThrowException()
        {
            var text = "123456789";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var result = reader.ReadTo(out var needle, "Test", null);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void StreamReader_ReadTo_OneNeedleIsEmpty_ShouldThrowException()
        {
            var text = "123456789";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            using (var reader = new StreamReader(stream))
            {
                var result = reader.ReadTo(out var needle, "", "Test");
            }
        }

        #endregion
    }
}
