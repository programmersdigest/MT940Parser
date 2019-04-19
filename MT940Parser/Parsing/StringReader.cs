using System;

namespace programmersdigest.MT940Parser.Parsing
{
    internal class StringReader
    {
        private string _buffer;
        private int _position;

        public StringReader(string buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            _buffer = buffer;
        }

        public string Read(int length = 0)
        {
            var result = Peek(length);

            _position += result.Length;

            return result;
        }

        public string ReadWhile(Func<char, bool> predicate, int length = 0)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "Length must be greater than or equal to zero.");
            }

            var start = _position;

            var end = length > 0
                    ? start + length
                    : _buffer.Length;

            if (end > _buffer.Length)
            {
                end = _buffer.Length;
            }

            for (var i = start; i < end; i++)
            {
                if (!predicate(_buffer[i]))
                {
                    end = i;
                    break;
                }
            }

            var take = end - start;
            _position += take;
            return _buffer.Substring(start, take);
        }

        public string Peek(int length = 0)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "Length must be greater than or equal to zero.");
            }

            var start = _position;

            var end = length > 0
                    ? start + length
                    : _buffer.Length;

            if (end > _buffer.Length)
            {
                end = _buffer.Length;
            }

            return _buffer.Substring(start, end - start);
        }

        public void Skip(int length)
        {
            var newPosition = _position + length;
            if (newPosition < 0)
                _position = 0;
            else if (newPosition > _buffer.Length)
                _position = _buffer.Length;
            else
                _position = newPosition;
        }
    }
}
