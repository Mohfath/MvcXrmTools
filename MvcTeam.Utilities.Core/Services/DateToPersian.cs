using System;

namespace MvcTeam.Utilities.Services
{
    public static class DateToPersian
    {
        public static string GetPersianDate(DateTime? date)
        {
            if(!date.HasValue)
            return "";

            var pc = new System.Globalization.PersianCalendar();
            return $"{pc.GetYear(date.Value)}/{pc.GetMonth(date.Value)}/{pc.GetDayOfMonth(date.Value)}";
        }
    }
}
