using System;

namespace Mohajer.Core
{
    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek = DayOfWeek.Saturday)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }
            return dt.AddDays(-1 * diff).Date;
        }

        public static int DayOfWeekNumber1Based(this DateTime dateTime)
        {
            var result = (int)dateTime.DayOfWeek + 2;

            if (result == 8) return 0;

            return result;
        }
    }
}