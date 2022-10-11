using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ApiShared.Core.Dates
{
    public abstract class IvDateInfo
    {
        public abstract string[] Days { get; }
        public abstract string[] Months { get; }
        public abstract string AM { get; }
        public abstract string PM { get; }

        public Calendar Cal { get; protected set; }
        public int MaxYear { get; protected set; }
        public int MaxMonth { get; protected set; }
        public int MaxDay { get; protected set; }
        public int MinYear { get; protected set; }
        public int MinMonth { get; protected set; }
        public int MinDay { get; protected set; }

        public void LoadCalendarValues()
        {
            DateTime dt = Cal.MaxSupportedDateTime;
            MaxYear = Cal.GetYear(dt);
            MaxMonth = Cal.GetMonth(dt);
            MaxDay = Cal.GetDayOfMonth(dt);

            dt = Cal.MinSupportedDateTime;
            MinYear = Cal.GetYear(dt);
            MinMonth = Cal.GetMonth(dt);
            MinDay = Cal.GetDayOfMonth(dt);
        }
    }

    public class VDateInfoPersian : IvDateInfo
    {
        private string[] _days;
        private string[] months;

        public VDateInfoPersian()
        {
            _days = new string[] { "یکشنبه", "دوشنبه", "سه شنبه", "چهارشنبه", "پنجشنبه", "جمعه", "شنبه" };
            months = new string[] { "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند" };

            Cal = new PersianCalendar();
            LoadCalendarValues();
        }

        public override string[] Days
        {
            get { return _days; }
        }

        public override string[] Months
        {
            get { return months; }
        }

        public override string AM
        {
            get { return "ق.ظ"; }
        }

        public override string PM
        {
            get { return "ب.ظ"; }
        }
    }

    public class VDateInfoArabic : IvDateInfo
    {
        private string[] days;
        private string[] months;

        public VDateInfoArabic() : this(null)
        {

        }

        public VDateInfoArabic(int? adjustDate)
        {
            days = new string[] { "اِلأَحَّد", "اِلأِثنين", "اِثَّلاثا", "اِلأَربِعا", "اِلخَميس", "اِجُّمعَة", "اِسَّبِت" };
            months = new string[] { "محرم", "صفر", "ربيع الاول", "ربيع الثاني", "جمادي الاول", "جمادي الثاني", "رجب", "شعبان", "رمضان", "شوال", "ذي القعدة", "ذي الحجة" };

            HijriCalendar cal = new HijriCalendar();
            if (adjustDate.HasValue)
                cal.HijriAdjustment = adjustDate.Value;
            Cal = cal;
            LoadCalendarValues();
        }
        public override string[] Days
        {
            get { return days; }
        }

        public override string[] Months
        {
            get { return months; }
        }

        public override string AM
        {
            get { return "ص"; }
        }

        public override string PM
        {
            get { return "م"; }
        }
    }

    public class VDateInfoEnglish : IvDateInfo
    {
        private string[] days;
        private string[] months;

        public VDateInfoEnglish()
        {
            days = new string[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
            months = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

            Cal = new GregorianCalendar();
            LoadCalendarValues();
        }
        public override string[] Days
        {
            get { return days; }
        }

        public override string[] Months
        {
            get { return months; }
        }

        public override string AM
        {
            get { return "AM"; }
        }

        public override string PM
        {
            get { return "PM"; }
        }
    }

    public class VDateInfo
    {
        public enum Info
        {
            Persian,
            Arabic,
            English
        }

        private static Dictionary<Info, IvDateInfo> cache = new Dictionary<Info, IvDateInfo>();
        public static IvDateInfo GetInfo(Info dateInfo)
        {
            IvDateInfo result;
            switch (dateInfo)
            {
                case Info.Persian:
                    if (!cache.Keys.Contains(Info.Persian))
                        cache[Info.Persian] = new VDateInfoPersian();
                    result = cache[Info.Persian];
                    break;
                case Info.Arabic:
                    if (!cache.Keys.Contains(Info.Arabic))
                        cache[Info.Arabic] = new VDateInfoArabic();
                    result = cache[Info.Arabic];
                    break;
                case Info.English:
                    if (!cache.Keys.Contains(Info.English))
                        cache[Info.English] = new VDateInfoEnglish();
                    result = cache[Info.English];
                    break;
                default:
                    if (!cache.Keys.Contains(Info.Persian))
                        cache[Info.Persian] = new VDateInfoPersian();
                    result = cache[Info.Persian];
                    break;
            }
            return result;
        }
    }
}