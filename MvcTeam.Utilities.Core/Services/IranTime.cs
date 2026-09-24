using System;

namespace MvcTeam.Utilities.Services
{
    //CRM passes dates to workflow steps as UTC
    public static class IranTime
    {
        private static readonly TimeZoneInfo Zone = TimeZoneInfo.FindSystemTimeZoneById("Iran Standard Time");

        public static DateTime FromUtc(DateTime value)
        {
            var utc = value.Kind == DateTimeKind.Local ? value.ToUniversalTime() : DateTime.SpecifyKind(value, DateTimeKind.Utc);
            return TimeZoneInfo.ConvertTimeFromUtc(utc, Zone);
        }

        public static DateTime ToUtc(DateTime iranTime)
        {
            return TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(iranTime, DateTimeKind.Unspecified), Zone);
        }
    }
}
