using System;

namespace ApiShared.Core.Dates
{
    public class CalculateDateDiff
    {
        public static DateDiff Calculate(DateDiff start, DateDiff end)
        {
            return Calculate(start, end, 0);
        }
        public static DateDiff Calculate(DateDiff start, DateDiff end, byte add)
        {
            return Calculate(new VDate(start.Year, start.Month, start.Days), new VDate(end.Year, end.Month, end.Days), add);
        }
        public static DateDiff Calculate(VDate startDate, VDate endDate)
        {
            return Calculate(startDate, endDate, 0);
        }
        public static DateDiff Calculate(DateTime startDate, DateTime endDate)
        {
            return Calculate(startDate, endDate, 0);
        }
        public static DateDiff Calculate(DateTime startDate, DateTime endDate, byte add)
        {
            return Calculate(new VDate(startDate), new VDate(endDate), add);
        }
        public static DateDiff Calculate(VDate startDate, VDate endDate, byte add)
        {
            var total = new DateDiff();
            if (startDate > endDate)
                return total;

            if (startDate == endDate)
                return total;
            total.Month = 0;

            if (!(startDate.Year == endDate.Year && startDate.Month == endDate.Month))
            {
                int nextMonth = startDate.Month, nextYear = startDate.Year;
                while (nextYear < endDate.Year || nextYear == endDate.Year && nextMonth <= endDate.Month)
                {
                    nextMonth = nextMonth + 1;
                    if (nextMonth > 12)
                    {
                        nextYear += 1;
                        nextMonth = 1;
                    }
                    total.Month += 1;
                }
                total.Month--;
            }


            if (startDate.Day <= endDate.Day)
            {
                total.Days = endDate.Day - startDate.Day + add;
            }
            else if (startDate.Day > endDate.Day)
            {
                endDate.Month--;
                total.Days = endDate.Day + (endDate.MonthLength - startDate.Day) + add;
            }

            total.Year = Convert.ToInt32(Math.Floor(Convert.ToSingle(total.Month) / 12));
            total.Month = total.Month - 12 * total.Year;

            return total;
        }
        public static int Age(VDate birthdate)
        {
            var year = birthdate.Year;
            var month = birthdate.Month;
            var day = birthdate.Day;

            var today = VDate.Now;
            var cYear = today.Year;
            var cMonth = today.Month;
            var cDay = today.Day;

            if (year > cYear)
                throw new Exception("Invalid Year, Must lower than this year");

            var age = cYear - year;
            if (cMonth < month)
                age--;
            else if (cMonth == month && cDay < day)
                age--;

            return age;
        }
        public static int Age(DateTime birthdate)
        {
            return Age(new VDate(birthdate));
        }
    }
}
