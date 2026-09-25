using System;

namespace MvcTeam.Utilities.Services
{
    //CRM passes dates to workflow steps as UTC.
    //Iran stopped daylight saving in 2022: the last period ended at 24:00 local time on 21 September 2022 (19:30 UTC).
    //From then on the offset is always +03:30. That is applied here directly, so a server whose Windows time-zone data
    //never got the 2022 update (and still adds an hour every summer) gives the same answer as an up-to-date one.
    //Earlier dates still use the server's time-zone history, which has the daylight-saving years right.
    public static class IranTime
    {
        private static readonly Lazy<TimeZoneInfo> ServerZone = new Lazy<TimeZoneInfo>(() => TimeZoneInfo.FindSystemTimeZoneById("Iran Standard Time"));

        private static readonly DateTime FixedOffsetFromUtc = new DateTime(2022, 9, 21, 19, 30, 0, DateTimeKind.Utc);
        private static readonly TimeSpan FixedOffset = TimeSpan.FromMinutes(210);

        public static DateTime FromUtc(DateTime value)
        {
            return FromUtc(value, null);
        }

        //serverZone is the Iran zone data to use for dates before the fixed offset applies; null means this server's own
        public static DateTime FromUtc(DateTime value, TimeZoneInfo serverZone)
        {
            var utc = value.Kind == DateTimeKind.Local ? value.ToUniversalTime() : DateTime.SpecifyKind(value, DateTimeKind.Utc);
            if (utc >= FixedOffsetFromUtc)
                return DateTime.SpecifyKind(utc + FixedOffset, DateTimeKind.Unspecified);

            return TimeZoneInfo.ConvertTimeFromUtc(utc, serverZone ?? ServerZone.Value);
        }

        public static DateTime ToUtc(DateTime iranTime)
        {
            return ToUtc(iranTime, null);
        }

        public static DateTime ToUtc(DateTime iranTime, TimeZoneInfo serverZone)
        {
            var local = DateTime.SpecifyKind(iranTime, DateTimeKind.Unspecified);
            var utc = DateTime.SpecifyKind(local - FixedOffset, DateTimeKind.Utc);
            if (utc >= FixedOffsetFromUtc)
                return utc;

            return TimeZoneInfo.ConvertTimeToUtc(local, serverZone ?? ServerZone.Value);
        }
    }
}
