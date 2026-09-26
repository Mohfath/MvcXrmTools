using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;

namespace MvcTeam.Utilities.Workflows.Common
{
    /// <summary>
    /// Class containing common functionality relating to business minutes
    /// </summary>
    public static class BusinessMinuteLogic
    {
        /// <summary>
        /// Method to check if a minute is part of a business day (i.e. Monday - Friday and not a holiday)
        /// </summary>
        /// <param name="dateToCheck">Date to evaluate</param>
        /// <param name="calendar">The holiday/closure calendar to use</param>
        /// <returns>True if the day is a business day</returns>
        public static bool IsBusinessMinute(this DateTime dateToCheck, Entity calendar)
        {
            var validBusinessDay = dateToCheck.DayOfWeek != DayOfWeek.Saturday && dateToCheck.DayOfWeek != DayOfWeek.Sunday;

            if (!validBusinessDay)
                return false;

            if (calendar == null)
                return true;

            var calendarRules = calendar.GetAttributeValue<EntityCollection>("calendarrules");

            foreach (var calendarRule in calendarRules.Entities)
            {
                // Date is not stored as UTC
                var startTime = calendarRule.GetAttributeValue<DateTime>("starttime");
                // Subtract 1 so the last minute is not double counted 
                // 4/2/2018 12:00 AM + 1440 minutes = 4/3/2018 12:00 AM - so the last minute is handled twice
                var duration = calendarRule.GetAttributeValue<int>("duration") - 1;
                var endTime = startTime.AddMinutes(duration);

                // Date is during a holiday
                if (dateToCheck.IsBetween(startTime, endTime))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Counts the business minutes from start to end, both included. It gives the same answer as calling
        /// IsBusinessMinute for every minute in between, but works a day at a time. Both times must be whole minutes.
        /// </summary>
        public static int CountBusinessMinutes(DateTime start, DateTime end, Entity calendar)
        {
            if (end < start)
                return 0;

            //The closed periods as whole minutes: a minute is closed when it lies between a rule's start and its start plus (duration - 1) minutes
            var closed = new List<KeyValuePair<DateTime, DateTime>>();
            if (calendar != null)
            {
                foreach (var rule in calendar.GetAttributeValue<EntityCollection>("calendarrules").Entities)
                {
                    var ruleStart = rule.GetAttributeValue<DateTime>("starttime");
                    var ruleEnd = ruleStart.AddMinutes(rule.GetAttributeValue<int>("duration") - 1);
                    var first = RoundUpToMinute(ruleStart);
                    var last = new DateTime(ruleEnd.Year, ruleEnd.Month, ruleEnd.Day, ruleEnd.Hour, ruleEnd.Minute, 0);
                    if (last >= first)
                        closed.Add(new KeyValuePair<DateTime, DateTime>(first, last));
                }
                closed.Sort((a, b) => a.Key.CompareTo(b.Key));
            }

            var total = 0;
            for (var day = start.Date; day <= end.Date; day = day.AddDays(1))
            {
                if (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday)
                    continue;

                var from = day > start ? day : start;
                var lastMinuteOfDay = day.AddDays(1).AddMinutes(-1);
                var to = lastMinuteOfDay < end ? lastMinuteOfDay : end;
                var minutes = (int)(to - from).TotalMinutes + 1;

                //Take off this day's closed minutes, counting overlapping periods once
                var coveredUpTo = from.AddMinutes(-1);
                foreach (var period in closed)
                {
                    if (period.Value < from || period.Key > to)
                        continue;

                    var periodFrom = period.Key > from ? period.Key : from;
                    var periodTo = period.Value < to ? period.Value : to;
                    if (periodFrom <= coveredUpTo)
                        periodFrom = coveredUpTo.AddMinutes(1);
                    if (periodTo >= periodFrom)
                    {
                        minutes -= (int)(periodTo - periodFrom).TotalMinutes + 1;
                        coveredUpTo = periodTo;
                    }
                }

                total += minutes;
            }

            return total;
        }

        private static DateTime RoundUpToMinute(DateTime time)
        {
            var down = new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, 0);
            return down == time ? time : down.AddMinutes(1);
        }
        /// <summary>
        /// Extension method to check if a point in time falls between 2 others (inclusive)
        /// </summary>
        /// <param name="input">Time to check</param>
        /// <param name="date1">Start of the time window</param>
        /// <param name="date2">End of the time window</param>
        /// <returns>True if the input is in the range or on the bounds</returns>
        private static bool IsBetween(this DateTime input, DateTime date1, DateTime date2)
        {
            return input >= date1 && input <= date2;
        }
    }
}