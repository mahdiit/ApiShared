using System;
using System.Text.RegularExpressions;

namespace ApiShared.Core.Dates
{
    public class VDate
    {
        private int _year, _day, _month, _hour, _minute, _seconds, _milisecond, _dayofWeek;
        public IvDateInfo DateInfo;

        private void SetDate(DateTime dt)
        {
            _year = DateInfo.Cal.GetYear(dt);
            _month = DateInfo.Cal.GetMonth(dt);
            _day = DateInfo.Cal.GetDayOfMonth(dt);
            _dayofWeek = (int)DateInfo.Cal.GetDayOfWeek(dt);
            _hour = dt.Hour;
            _minute = dt.Minute;
            _seconds = dt.Second;
            MiliSeconds = dt.Millisecond;
        }
        private static void FixOne(ref int one, int unit, ref int next)
        {
            if (one < 0 || one >= unit)
            {
                if (one < 0)
                {
                    while (one < 0)
                    {
                        one += unit;
                        next--;
                    }
                }
                else
                {
                    while (one >= unit)
                    {
                        one -= unit;
                        next++;
                    }
                }
            }
        }
        private void FixMonthYear(ref int year, ref int month)
        {
            if (year > DateInfo.MaxYear || year < DateInfo.MinYear)
            {
                throw new Exception("مقدار سال نا معتبر است");
            }
            int maxMonth = DateInfo.Cal.GetMonthsInYear(year);
            if (month < 1 || month > maxMonth)
            {
                if (month < 1)
                {
                    while (month < 1)
                    {
                        year--;
                        maxMonth = DateInfo.Cal.GetMonthsInYear(year);
                        month += maxMonth;
                    }
                }
                else
                {
                    while (month > maxMonth)
                    {
                        year++;
                        maxMonth = DateInfo.Cal.GetMonthsInYear(year);
                        month -= maxMonth;
                    }
                }
            }
            if (year > DateInfo.MaxYear || year < DateInfo.MinYear)
            {
                throw new Exception("مقادیر سال و ماه نامعتبر است");
            }
        }
        private void FixAll(ref int year, ref int month, ref int day, ref int hour, ref int minute, ref int second, ref int milisecond)
        {
            FixOne(ref milisecond, 1000, ref second);
            FixOne(ref second, 60, ref minute);
            FixOne(ref minute, 60, ref hour);
            FixOne(ref hour, 24, ref day);
            FixMonthYear(ref year, ref month);
            int maxDay = DateInfo.Cal.GetDaysInMonth(year, month);
            if (IsLastDayOfMonth && OnChangeSaveLastDay)
                day = maxDay;

            if (day < 1 || day > maxDay)
            {
                if (day < 1)
                {
                    while (day < 1)
                    {
                        month--;
                        FixMonthYear(ref year, ref month);
                        day += maxDay;
                        maxDay = DateInfo.Cal.GetDaysInMonth(year, month);
                    }
                }
                else
                {
                    while (day > maxDay)
                    {
                        month++;
                        FixMonthYear(ref year, ref month);
                        day -= maxDay;
                        maxDay = DateInfo.Cal.GetDaysInMonth(year, month);
                    }
                }
            }
            FixMonthYear(ref year, ref month);
            _dayofWeek = (int)DateInfo.Cal.GetDayOfWeek(ToDateTime());
        }
        private static string TwoDigit(int input)
        {
            string twoDJ = "";
            int len = input.ToString().Length;
            if (len == 1)
                twoDJ = input.ToString().Insert(0, "0");
            else if (len == 2)
                twoDJ = input.ToString();
            else if (len > 2)
            {

            }
            return twoDJ;
        }

        private string Format(string expression, bool usePersianNo = false)
        {
            UsePersianNumbers = usePersianNo;

            switch (expression)
            {
                case "F":
                case "f":
                    expression = Format("$dddd, $d $MMMM $yyyy $HH:$mm:$ss $g");
                    break;
                case "S":
                case "s":
                    expression = Format("$yyyy/$MM/$dd $HH:$mm:$ss $g");
                    break;
                default:
                    expression = Regex.Replace(expression, "\\$d{4}", DateInfo.Days[DayOfWeek]);
                    expression = Regex.Replace(expression, "\\$d{2}", TwoDigit(Day));
                    expression = Regex.Replace(expression, "\\$d{1}", Day.ToString());

                    expression = Regex.Replace(expression, "\\$M{4}", DateInfo.Months[Month - 1]);
                    expression = Regex.Replace(expression, "\\$M{2}", TwoDigit(Month));
                    expression = Regex.Replace(expression, "\\$M{1}", Month.ToString());

                    expression = Regex.Replace(expression, "\\$y{4}", Year.ToString());
                    expression = Regex.Replace(expression, "\\$y{2}", TwoDigitYear.ToString());

                    expression = Regex.Replace(expression, "\\$H{2}", TwoDigit(Hour));
                    expression = Regex.Replace(expression, "\\$H{1}", Hour.ToString());

                    expression = Regex.Replace(expression, "\\$h{2}", TwoDigit(Hour12));
                    expression = Regex.Replace(expression, "\\$h{1}", Hour12.ToString());

                    expression = Regex.Replace(expression, "\\$m{2}", TwoDigit(Minute));
                    expression = Regex.Replace(expression, "\\$m{1}", Minute.ToString());

                    expression = Regex.Replace(expression, "\\$s{2}", TwoDigit(Seconds));
                    expression = Regex.Replace(expression, "\\$s{1}", Seconds.ToString());

                    expression = Regex.Replace(expression, "\\$g{1}", Daylight);
                    break;
            }
            return UsePersianNumbers ? expression.ReplaceNo() : expression;
        }

        #region Constructors
        public VDate(int year, int month, int day)
            : this(year, month, day, 0, 0, 0, 0, VDateInfo.GetInfo(VDateInfo.Info.Persian))
        {
        }
        public VDate(int year, int month, int day, IvDateInfo DInfo)
            : this(year, month, day, 0, 0, 0, 0, DInfo)
        {
        }
        public VDate(int year, int month, int day, int hour, int minute, int second, int milisecond)
            : this(year, month, day, hour, minute, second, milisecond, VDateInfo.GetInfo(VDateInfo.Info.Persian))
        {
        }
        public VDate(int year, int month, int day, int hour, int minute, int second, int milisecond, IvDateInfo dInfo)
        {
            DateInfo = dInfo;
            _year = year;
            _month = month;
            _day = day;
            _hour = hour;
            _minute = minute;
            _seconds = second;
            MiliSeconds = milisecond;
            UsePersianNumbers = false;
        }
        public VDate()
            : this(VDateInfo.GetInfo(VDateInfo.Info.Persian), DateTime.Now)
        {
        }
        public VDate(DateTime dt)
            : this(VDateInfo.GetInfo(VDateInfo.Info.Persian), dt)
        {
        }
        public VDate(IvDateInfo DInfo)
            : this(DInfo, DateTime.Now)
        {
        }
        public VDate(IvDateInfo DInfo, DateTime dt)
        {
            DateInfo = DInfo;
            UsePersianNumbers = false;
            SetDate(dt);
        }
        #endregion

        #region Public Properties
        public bool OnChangeSaveLastDay { get; set; }
        private bool IsLastDayOfMonth { get; set; }

        public bool UsePersianNumbers { get; set; }
        public int Day
        {
            get { return _day; }
            set
            {
                IsLastDayOfMonth = DateInfo.Cal.GetDaysInMonth(Year, Month) == Day;
                _day = value;
                FixAll(ref _year, ref _month, ref _day, ref _hour, ref _minute, ref _seconds, ref _milisecond);
            }
        }
        public int DayOfWeek
        {
            get
            {
                return _dayofWeek;
            }
        }
        public int Month
        {
            get { return _month; }
            set
            {
                IsLastDayOfMonth = DateInfo.Cal.GetDaysInMonth(Year, Month) == Day;
                _month = value;
                FixAll(ref _year, ref _month, ref _day, ref _hour, ref _minute, ref _seconds, ref _milisecond);
            }
        }
        public int Year
        {
            get { return _year; }
            set
            {
                IsLastDayOfMonth = DateInfo.Cal.GetDaysInMonth(Year, Month) == Day;
                _year = value;
                FixAll(ref _year, ref _month, ref _day, ref _hour, ref _minute, ref _seconds, ref _milisecond);
            }
        }
        public int Hour
        {
            get { return _hour; }
            set
            {
                _hour = value;
                FixAll(ref _year, ref _month, ref _day, ref _hour, ref _minute, ref _seconds, ref _milisecond);
            }
        }
        public int Hour12
        {
            get
            {
                if (_hour > 12)
                    return _hour - 12;
                else
                    return _hour;
            }
        }
        public int Minute
        {
            get { return _minute; }
            set
            {
                _minute = value;
                FixAll(ref _year, ref _month, ref _day, ref _hour, ref _minute, ref _seconds, ref _milisecond);
            }
        }
        public int Seconds
        {
            get { return _seconds; }
            set
            {
                _seconds = value;
                FixAll(ref _year, ref _month, ref _day, ref _hour, ref _minute, ref _seconds, ref _milisecond);
            }
        }
        public VDate SetPersianNumber(bool newVal)
        {
            UsePersianNumbers = newVal;
            return this;
        }
        public int MiliSeconds
        {
            get { return _milisecond; }
            set
            {
                _milisecond = value;
                FixAll(ref _year, ref _month, ref _day, ref _hour, ref _minute, ref _seconds, ref _milisecond);
            }
        }
        public bool IsLeapYear
        {
            get
            {
                return DateInfo.Cal.IsLeapYear(Year);
            }
        }
        public bool IsLeapMonth
        {
            get
            {
                return DateInfo.Cal.IsLeapMonth(Year, Month);
            }
        }
        public bool IsLeapDay
        {
            get
            {
                return DateInfo.Cal.IsLeapDay(Year, Month, Day);
            }
        }
        public int TwoDigitYear
        {
            get
            {
                if (Year.ToString().Length == 4)
                    return Convert.ToInt32(Year.ToString().Substring(2, 2));
                else
                    return Year;
            }
        }
        public string Daylight
        {
            get
            {
                if (Hour > 12)
                    return DateInfo.PM;
                else
                    return DateInfo.AM;
            }
        }
        public int MonthLength
        {
            get
            {
                return DateInfo.Cal.GetDaysInMonth(Year, Month);
            }
        }
        #endregion

        public DateTime ToDateTime()
        {
            return new DateTime(Year, Month, Day, Hour, Minute, Seconds, MiliSeconds, DateInfo.Cal);
        }

        public string ToString(bool usePersianNo)
        {
            return Format("s", usePersianNo);
        }
        public string ToString(string format, bool usePersianNo)
        {
            return Format(format, usePersianNo);
        }

        public new string ToString()
        {
            return ToString(false);
        }

        public string ToString(string format)
        {
            return ToString(format, false);
        }

        public override int GetHashCode()
        {
            return ToDateTime().GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return ToDateTime().Equals(obj);
        }
        public static VDate Now
        {
            get { return new VDate(); }
        }
        public static VDate Today
        {
            get { return new VDate(DateTime.Today); }
        }

        private const string RegexDate = "\\A(\\d{4})[\\\\/\\-](\\d{1,2})[\\\\/\\-](\\d{1,2})\\z";
        private const string RegexDateTime = "\\A(\\d{4})[\\\\/\\-](\\d{1,2})[\\\\/\\-](\\d{1,2})\\s(\\d{1,2}):(\\d{1,2}):(\\d{1,2})\\z";
        private const string RegexDateTimeFactor = "\\A(\\d{4})\\/(\\d{1,2})\\/(\\d{1,2})\\-(\\d{1,2}):(\\d{1,2})\\z";
        public static bool TryParse(string dt, out VDate result)
        {
            bool HasResult = false;
            result = null;
            try
            {
                if (Regex.IsMatch(dt, RegexDate))
                {
                    Regex rg = new Regex(RegexDate);
                    int y, m, d;
                    Match r = rg.Match(dt);
                    y = Convert.ToInt32(r.Groups[1].Value);
                    m = Convert.ToInt32(r.Groups[2].Value);
                    d = Convert.ToInt32(r.Groups[3].Value);
                    result = new VDate(y, m, d);
                    HasResult = true;
                }
                else if (Regex.IsMatch(dt, RegexDateTime))
                {
                    Regex rg = new Regex(RegexDateTime);
                    int y, m, d, h, min, s;

                    Match r = rg.Match(dt);
                    y = Convert.ToInt32(r.Groups[1].Value);
                    m = Convert.ToInt32(r.Groups[2].Value);
                    d = Convert.ToInt32(r.Groups[3].Value);
                    h = Convert.ToInt32(r.Groups[4].Value);
                    min = Convert.ToInt32(r.Groups[5].Value);
                    s = Convert.ToInt32(r.Groups[6].Value);
                    result = new VDate(y, m, d, h, min, s, 0);
                    HasResult = true;
                }
                else if (Regex.IsMatch(dt, RegexDateTimeFactor))
                {
                    Regex rg = new Regex(RegexDateTimeFactor);
                    int y, m, d, h, min, s;

                    Match r = rg.Match(dt);
                    y = Convert.ToInt32(r.Groups[1].Value);
                    m = Convert.ToInt32(r.Groups[2].Value);
                    d = Convert.ToInt32(r.Groups[3].Value);
                    h = Convert.ToInt32(r.Groups[4].Value);
                    min = Convert.ToInt32(r.Groups[5].Value);
                    s = 0;
                    result = new VDate(y, m, d, h, min, s, 0);
                    HasResult = true;
                }
            }
            catch { }

            return HasResult;
        }

        public static bool operator >(VDate a, VDate b)
        {
            return a.ToDateTime() > b.ToDateTime();
        }
        public static bool operator <(VDate a, VDate b)
        {
            return a.ToDateTime() < b.ToDateTime();
        }

        public static bool operator <=(VDate a, VDate b)
        {
            return a.ToDateTime() <= b.ToDateTime();
        }
        public static bool operator >=(VDate a, VDate b)
        {
            return a.ToDateTime() >= b.ToDateTime();
        }

        public static bool operator ==(VDate a, VDate b)
        {
            if (a == (object)b) return true;
            if ((object)a == null || (object)b == null) return false;
            return a.ToDateTime() == b.ToDateTime();
        }

        public static bool operator !=(VDate a, VDate b)
        {
            return !(a == b);
        }

        public static explicit operator int(VDate input)
        {
            return Convert.ToInt32(string.Format("{0}{1}{2}",
                    input.Year.ToString("D4"),
                    input.Month.ToString("D2"),
                    input.Day.ToString("D2")));
        }

        public static explicit operator VDate(int input)
        {
            string dtStr = input.ToString();
            if (dtStr.Length == 8)
                return new VDate(
                    Convert.ToInt32(dtStr.Substring(0, 4)),
                    Convert.ToInt32(dtStr.Substring(4, 2)),
                    Convert.ToInt32(dtStr.Substring(6, 2))
                );

            return Now;
        }
    }
}
