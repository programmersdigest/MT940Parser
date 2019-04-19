using Microsoft.VisualStudio.TestTools.UnitTesting;
using programmersdigest.MT940Parser.Parsing;
using System;

namespace MT940ParserTests
{
    [TestClass]
    public class StringReaderTests
    {
        #region Ctor

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StringReader_Ctor_BufferIsNull_ShouldThrowArgumentNullException()
        {
            string buffer = null;
            var reader = new StringReader(buffer);
        }

        [TestMethod]
        public void StringReader_Ctor_BufferIsEmpty_ShouldInitialize()
        {
            var buffer = "";
            var reader = new StringReader(buffer);

            Assert.IsNotNull(reader);
        }

        #endregion

        #region Skip()

        [TestMethod]
        public void StringReader_Skip_BufferIsEmpty_ShouldSkipToEnd()
        {
            var buffer = "";
            var reader = new StringReader(buffer);

            reader.Skip(10);
            var value = reader.Read();

            Assert.AreEqual("", value);
        }

        [TestMethod]
        public void StringReader_Skip_BufferHasMoreChars_ShouldSkipGivenNumberOfChars()
        {
            var buffer = "1234567890";
            var reader = new StringReader(buffer);

            reader.Skip(5);
            var value = reader.Read();

            Assert.AreEqual("67890", value);
        }

        [TestMethod]
        public void StringReader_Skip_SkipOverEndOfBuffer_ShouldSkipToEnd()
        {
            var buffer = "1234567890";
            var reader = new StringReader(buffer);

            reader.Skip(11);
            var value = reader.Read();

            Assert.AreEqual("", value);
        }

        [TestMethod]
        public void StringReader_Skip_Negative_ShouldSkipGivenNumberOfChars()
        {
            var buffer = "1234567890";
            var reader = new StringReader(buffer);
            reader.Skip(5);

            reader.Skip(-3);
            var value = reader.Read();

            Assert.AreEqual("34567890", value);
        }

        [TestMethod]
        public void StringReader_Skip_NegativeBeforeBeginning_ShouldSkipToBeginning()
        {
            var buffer = "1234567890";
            var reader = new StringReader(buffer);
            reader.Skip(5);

            reader.Skip(-10);
            var value = reader.Read();

            Assert.AreEqual("1234567890", value);
        }

        #endregion

        #region Peek()

        [TestMethod]
        public void StringReader_Peek_NoParam_ShouldReturnWholeBuffer()
        {
            var buffer = "1234567890";
            var reader = new StringReader(buffer);

            var value = reader.Peek();

            Assert.AreEqual("1234567890", value);
        }

        [TestMethod]
        public void StringReader_Peek_NoParam_MiddleOfBuffer_ShouldReturnRestOfBuffer()
        {
            var buffer = "1234567890";
            var reader = new StringReader(buffer);
            reader.Skip(5);

            var value = reader.Peek();

            Assert.AreEqual("67890", value);
        }

        [TestMethod]
        public void StringReader_Peek_NoParam_ShouldNotMovePosition()
        {
            var buffer = "1234567890";
            var reader = new StringReader(buffer);

            reader.Peek();
            reader.Peek();
            reader.Peek();
            reader.Peek();

            var value = reader.Read();
            Assert.AreEqual("1234567890", value);
        }

        [TestMethod]
        public void StringReader_Peek_Param_ShouldPeekGivenNumOfChars()
        {
            var buffer = "1234567890";
            var reader = new StringReader(buffer);

            var value = reader.Peek(5);

            Assert.AreEqual("12345", value);
        }

        [TestMethod]
        public void StringReader_Peek_Param_MiddleOfBuffer_ShouldPeekGivenNumOfChars()
        {
            var buffer = "1234567890";
            var reader = new StringReader(buffer);
            reader.Skip(3);

            var value = reader.Peek(3);

            Assert.AreEqual("456", value);
        }

        [TestMethod]
        public void StringReader_Peek_ParamOverLengthOfBuffer_ShouldPeekToEndOfBuffer()
        {
            var buffer = "1234567890";
            var reader = new StringReader(buffer);

            var value = reader.Peek(20);

            Assert.AreEqual("1234567890", value);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void StringReader_Peek_NegativeParam_ShouldThrowArgumentOutOfRangeException()
        {
            var buffer = "1234567890";
            var reader = new StringReader(buffer);
            reader.Skip(10);

            reader.Peek(-5);
        }

        [TestMethod]
        public void StringReader_Peek_ParamZero_ShouldBehaveLikeNoParam()
        {
            var buffer = "1234567890";
            var reader = new StringReader(buffer);

            var value = reader.Peek(0);

            Assert.AreEqual("1234567890", value);
        }

        #endregion

        #region Read()

        [TestMethod]
        public void StringReader_Read_NoParam_ShouldReturnWholeBuffer()
        {
            var buffer = "1234567890";
            var reader = new StringReader(buffer);

            var value = reader.Read();

            Assert.AreEqual("1234567890", value);
        }

        [TestMethod]
        public void StringReader_Read_NoParam_MiddleOfBuffer_ShouldReturnRestOfBuffer()
        {
            var buffer = "1234567890";
            var reader = new StringReader(buffer);
            reader.Skip(5);

            var value = reader.Read();

            Assert.AreEqual("67890", value);
        }

        [TestMethod]
        public void StringReader_Read_NoParam_ShouldMovePosition()
        {
            var buffer = "1234567890";
            var reader = new StringReader(buffer);

            reader.Read(3);
            reader.Read(3);
            var value = reader.Read(3);

            Assert.AreEqual("789", value);
        }

        [TestMethod]
        public void StringReader_Read_Param_ShouldReturnGivenNumOfChars()
        {
            var buffer = "1234567890";
            var reader = new StringReader(buffer);

            var value = reader.Read(5);

            Assert.AreEqual("12345", value);
        }

        [TestMethod]
        public void StringReader_Read_Param_MiddleOfBuffer_ShouldReturnGivenNumOfChars()
        {
            var buffer = "1234567890";
            var reader = new StringReader(buffer);
            reader.Skip(3);

            var value = reader.Read(3);

            Assert.AreEqual("456", value);
        }

        [TestMethod]
        public void StringReader_Read_ParamOverLengthOfBuffer_ShouldReturnAllToEndOfBuffer()
        {
            var buffer = "1234567890";
            var reader = new StringReader(buffer);

            var value = reader.Read(20);

            Assert.AreEqual("1234567890", value);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void StringReader_Read_NegativeParam_ShouldThrowArgumentOutOfRangeException()
        {
            var buffer = "1234567890";
            var reader = new StringReader(buffer);
            reader.Skip(10);

            reader.Read(-5);
        }

        [TestMethod]
        public void StringReader_Read_ParamZero_ShouldBehaveLikeNoParam()
        {
            var buffer = "1234567890";
            var reader = new StringReader(buffer);

            var value = reader.Read(0);

            Assert.AreEqual("1234567890", value);
        }

        #endregion

        #region ReadWhile()

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StringReader_ReadWhile_PredicateIsNull_ShouldThrowArgumentNullException()
        {
            var buffer = "12345abcde";
            var reader = new StringReader(buffer);

            reader.ReadWhile(null);
        }

        [TestMethod]
        public void StringReader_ReadWhile_PredicateNoLength_ShouldReturnAllMatchingChars()
        {
            var buffer = "12345abcde";
            var reader = new StringReader(buffer);

            var value = reader.ReadWhile(char.IsNumber);

            Assert.AreEqual("12345", value);
        }

        [TestMethod]
        public void StringReader_ReadWhile_PredicateNoLength_MiddleOfBuffer_ShouldReturnRestOfBuffer()
        {
            var buffer = "12345abcde";
            var reader = new StringReader(buffer);
            reader.Skip(3);

            var value = reader.ReadWhile(char.IsNumber);

            Assert.AreEqual("45", value);
        }

        [TestMethod]
        public void StringReader_ReadWhile_PredicateNoLength_ShouldMovePosition()
        {
            var buffer = "123abc456def";
            var reader = new StringReader(buffer);

            reader.ReadWhile(char.IsNumber);
            reader.ReadWhile(char.IsLetter);
            var value = reader.ReadWhile(char.IsNumber);

            Assert.AreEqual("456", value);
        }

        [TestMethod]
        public void StringReader_ReadWhile_PredicateAndLength_ShouldReturnGivenNumOfChars()
        {
            var buffer = "12345abcde";
            var reader = new StringReader(buffer);

            var value = reader.ReadWhile(char.IsNumber, 3);

            Assert.AreEqual("123", value);
        }

        [TestMethod]
        public void StringReader_ReadWhile_PredicateAndLength_MiddleOfBuffer_ShouldReturnGivenNumOfChars()
        {
            var buffer = "12345abcde";
            var reader = new StringReader(buffer);
            reader.Skip(3);

            var value = reader.ReadWhile(char.IsNumber, 5);

            Assert.AreEqual("45", value);
        }

        [TestMethod]
        public void StringReader_ReadWhile_PredicateAndLength_OverLengthOfBuffer_ShouldReturnAllToEndOfBuffer()
        {
            var buffer = "1234567890";
            var reader = new StringReader(buffer);

            var value = reader.ReadWhile(char.IsNumber, 20);

            Assert.AreEqual("1234567890", value);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void StringReader_ReadWhile_NegativeLength_ShouldThrowArgumentOutOfRangeException()
        {
            var buffer = "12345abcde";
            var reader = new StringReader(buffer);
            reader.Skip(10);

            reader.ReadWhile(char.IsLetter, -5);
        }

        [TestMethod]
        public void StringReader_ReadWhile_LengthZero_ShouldBehaveLikeNoParam()
        {
            var buffer = "12345abcde";
            var reader = new StringReader(buffer);

            var value = reader.ReadWhile(char.IsNumber, 0);

            Assert.AreEqual("12345", value);
        }

        #endregion
    }
}
