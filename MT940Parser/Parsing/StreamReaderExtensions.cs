using System;
using System.IO;
using System.Linq;
using System.Text;

namespace programmersdigest.MT940Parser.Parsing
{
    internal static class StreamReaderExtensions
    {
        internal static void Find(this StreamReader reader, string needle)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }
            if (needle == null)
            {
                throw new ArgumentNullException(nameof(needle));
            }
            if (needle == "")
            {
                throw new ArgumentOutOfRangeException(nameof(needle));
            }

            var needlePos = 0;

            while (!reader.EndOfStream)
            {
                var next = (char)reader.Read();

                // Check if character matches char in needle
                if (next == needle[needlePos])
                {
                    needlePos++;

                    if (needlePos == needle.Length)
                    {
                        return;
                    }
                }
                else
                {
                    // Character does not match
                    needlePos = 0;

                    // Check if character matches first char in needle to begin new search
                    if (next == needle[needlePos])
                    {
                        needlePos++;

                        if (needlePos == needle.Length)
                        {
                            return;
                        }
                    }
                }
            }
        }

        internal static string ReadTo(this StreamReader reader, out string matchedNeedle, params string[] needles)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }
            if (needles == null)
            {
                throw new ArgumentNullException(nameof(needles));
            }
            if (needles.Length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(needles), "Must provide at least one needle.");
            }
            if (needles.Any(n => string.IsNullOrEmpty(n)))
            {
                throw new ArgumentOutOfRangeException(nameof(needles), "Needle cannot be null or an empty string.");
            }


            var needlePositions = new int[needles.Length];
            var builder = new StringBuilder();

            matchedNeedle = null;

            while (!reader.EndOfStream && matchedNeedle == null)
            {
                var next = (char)reader.Read();
                builder.Append(next);

                matchedNeedle = CheckNeedles(next, needles, ref needlePositions);
            }

            if (matchedNeedle != null)
            {
                builder.Remove(builder.Length - matchedNeedle.Length, matchedNeedle.Length);    // Remove needle from result
            }

            return builder.ToString();
        }

        private static string CheckNeedles(char next, string[] needles, ref int[] needlePositions)
        {
            for (var i = 0; i < needles.Length; i++)
            {
                var needle = needles[i];

                if (next == needle[needlePositions[i]])
                {
                    // Character matches current needle position
                    needlePositions[i]++;

                    if (needlePositions[i] == needle.Length)
                    {
                        return needle;
                    }
                }
                else if (needlePositions[i] != 0)
                {
                    // Character does not match needle at current position.
                    // If we are not at position 0, try matching the needle
                    // starting from position 0. This handles edge cases like
                    // "NeeNeedle" where the actual match follows a partly match.
                    needlePositions[i] = 0;
                    i--;
                }
            }

            return null;
        }
    }
}
