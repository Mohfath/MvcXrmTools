using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MvcTeam.Utilities.Services
{
    public static class BusinessDayCalculator
    {
        //Moves forward (or backward when days is negative) by business days, skipping weekend days and holidays.
        //Holidays are compared by date only. With days = 0 the start date is returned as is.
        public static DateTime AddBusinessDays(DateTime start, int days, ISet<DayOfWeek> weekendDays, ISet<DateTime> holidays)
        {
            if (days != 0 && weekendDays.Count >= 7)
                throw new InvalidPluginExecutionException("Weekend Days can't include all seven days.");

            var step = days < 0 ? -1 : 1;
            var remaining = Math.Abs(days);
            var result = start;

            while (remaining > 0)
            {
                result = result.AddDays(step);

                if (weekendDays.Contains(result.DayOfWeek) || holidays.Contains(result.Date))
                    continue;

                remaining--;
            }

            return result;
        }

        //Counts business days from start to end, excluding the start day and including the end day,
        //so AddBusinessDays(start, n) = end gives back n. Negative when end is before start. Only dates matter, not times.
        public static int CountBusinessDays(DateTime start, DateTime end, ISet<DayOfWeek> weekendDays, ISet<DateTime> holidays)
        {
            var step = end.Date < start.Date ? -1 : 1;
            var day = start.Date;
            var count = 0;

            while (day != end.Date)
            {
                day = day.AddDays(step);
                if (!weekendDays.Contains(day.DayOfWeek) && !holidays.Contains(day)) count++;
            }
            return step * count;
        }

        //Holiday values from CRM (UTC or date-only) as Iran calendar dates
        public static HashSet<DateTime> ToIranDates(IEnumerable<DateTime> holidays)
        {
            return new HashSet<DateTime>(holidays.Select(h => IranTime.FromUtc(h).Date));
        }

        //"4|5" -> Thursday, Friday (0 = Sunday ... 6 = Saturday). Empty means no weekend days.
        public static HashSet<DayOfWeek> ParseWeekendDays(string value)
        {
            var result = new HashSet<DayOfWeek>();
            if (string.IsNullOrWhiteSpace(value)) return result;

            foreach (var part in value.Split('|'))
            {
                var text = part.Trim();
                if (text == "") continue;

                if (!int.TryParse(text, out var day) || day < 0 || day > 6)
                    throw new InvalidPluginExecutionException($"Weekend Days '{value}' is not valid. Use day numbers 0-6 separated by '|' (0 = Sunday, 6 = Saturday), e.g. '4|5'.");

                result.Add((DayOfWeek)day);
            }

            return result;
        }
    }
}
