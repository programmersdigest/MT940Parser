using System;
using System.Linq;

namespace programmersdigest.MT940Parser.Parsing
{
    public static class DateParser
    {
        public static DateTime Parse(string date)
        {
            if (date == null)
            {
                throw new ArgumentNullException(nameof(date), "Date must not be null");
            }
            if (date.Length != 6)
            {
                throw new FormatException("Date has to be given in the form yyMMdd");
            }
            if (!date.All(char.IsNumber))
            {
                throw new FormatException("Date must only contain numbers 0-9");
            }

            var year = int.Parse(date.Substring(0, 2));
            var month = int.Parse(date.Substring(2, 2));
            var day = int.Parse(date.Substring(4, 2));

            if (year > 79)
            {
                year += 1900;
            }
            else
            {
                year += 2000;
            }

            return new DateTime(year, month, day);
        }
    }
}
