using MD.PersianDateTime;
using Microsoft.Xrm.Sdk;
using System;
using System.Globalization;
using System.Text;

namespace MvcTeam.Utilities.Services
{
    //Formats a date in the Persian calendar using .NET-style custom format patterns:
    //  yyyy yy y   year        MMMM MMM month name, MM M month number
    //  dddd        day name    ddd short day name, dd d day of month
    //  HH H        24-hour     hh h 12-hour, mm m minute, ss s second, f..fffffff fraction
    //  tt t        ق.ظ / ب.ظ (or first letter)
    //  'text' "text" \c        literal text; any other character is copied as is
    //Doesn't depend on the server's culture settings.
    public static class PersianDateFormatter
    {
        public static string Format(DateTime value, string format, bool persianDigits)
        {
            var persian = new PersianDateTime(value);
            var output = new StringBuilder();
            var i = 0;

            while (i < format.Length)
            {
                var c = format[i];

                if (c == '\'' || c == '"')
                {
                    var end = format.IndexOf(c, i + 1);
                    if (end < 0) throw new InvalidPluginExecutionException($"Format '{format}' has an unclosed quote.");
                    output.Append(format, i + 1, end - i - 1);
                    i = end + 1;
                    continue;
                }
                if (c == '\\')
                {
                    if (i + 1 >= format.Length) throw new InvalidPluginExecutionException($"Format '{format}' ends with '\\'.");
                    output.Append(format[i + 1]);
                    i += 2;
                    continue;
                }
                if (c == '%')
                {
                    //.NET uses '%' to mark a single-letter custom format; nothing to output
                    i++;
                    continue;
                }

                var count = 1;
                while (i + count < format.Length && format[i + count] == c) count++;

                var token = Token(c, count, value, persian);
                if (token == null)
                {
                    output.Append(c, count);
                }
                else
                {
                    output.Append(persianDigits ? ToPersianDigits(token) : token);
                }
                i += count;
            }

            return output.ToString();
        }

        //Returns null when the character isn't a format letter
        private static string Token(char c, int count, DateTime value, PersianDateTime persian)
        {
            var hour12 = value.Hour % 12 == 0 ? 12 : value.Hour % 12;
            var amPm = value.Hour < 12 ? "ق.ظ" : "ب.ظ";

            switch (c)
            {
                case 'y':
                    if (count == 1) return Number(persian.Year % 100, 1);
                    if (count == 2) return Number(persian.Year % 100, 2);
                    return Number(persian.Year, count);
                case 'M':
                    return count >= 3 ? persian.GetLongMonthName : Number(persian.Month, count);
                case 'd':
                    if (count == 3) return persian.GetShortDayOfWeekName;
                    if (count >= 4) return persian.GetLongDayOfWeekName;
                    return Number(persian.Day, count);
                case 'H':
                    return Number(value.Hour, Math.Min(count, 2));
                case 'h':
                    return Number(hour12, Math.Min(count, 2));
                case 'm':
                    return Number(value.Minute, Math.Min(count, 2));
                case 's':
                    return Number(value.Second, Math.Min(count, 2));
                case 'f':
                    var digits = Math.Min(count, 7);
                    var fraction = value.Ticks % TimeSpan.TicksPerSecond;
                    return Number(fraction / (long)Math.Pow(10, 7 - digits), digits);
                case 't':
                    return count == 1 ? amPm.Substring(0, 1) : amPm;
                default:
                    return null;
            }
        }

        private static string Number(long value, int minDigits) =>
            value.ToString(new string('0', minDigits), CultureInfo.InvariantCulture);

        private static string ToPersianDigits(string text)
        {
            var result = new StringBuilder(text.Length);
            foreach (var c in text)
            {
                result.Append(c >= '0' && c <= '9' ? (char)('۰' + (c - '0')) : c);
            }
            return result.ToString();
        }
    }
}
