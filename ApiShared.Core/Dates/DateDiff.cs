using System.Collections.Generic;

namespace ApiShared.Core.Dates
{
    public class DateDiff
    {
        //public static char[] NumArr = new char[] { '٠', '١', '٢', '٣', '۴', '۵', '۶', '٧', '٨', '٩' };
        public int Year { get; set; }
        public int Month { get; set; }
        public int Days { get; set; }

        public override string ToString()
        {
            List<string> result = new List<string>();
            if (Year > 0)
                result.Add(Year + " سال");

            if (Month > 0)
                result.Add(Month + " ماه");

            if (Days > 0)
                result.Add(Days + " روز");

            string resultTest = string.Join(" و ", result.ToArray());
            //for (int i = 0; i < NumArr.Length; i++)
            //{
            //    resultTest = resultTest.Replace(i.ToString(), NumArr[i] + "");
            //}
            return resultTest.ReplaceNo();
        }
    }
}
