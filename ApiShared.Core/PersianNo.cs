using ApiShared.Core.Dates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

public static class PersianNo
{
    public static TimeSpan OneDay()
    {
        return new TimeSpan(1, 0, 0, 0);
    }

    private static readonly char[] NumArr = new char[] { '٠', '۱', '٢', '٣', '۴', '۵', '۶', '٧', '٨', '٩' };
    //private static readonly char[] NumArrOriginal = new char[] { '0','1', '2', '3', '4', '5', '6', '7', '8', '9' };

    public static string ToPersianNo(this int input)
    {
        string ch = input.ToString();
        return ch.Aggregate("", (current, t) => current + NumArr[Convert.ToByte(t + "")]);
    }

    public static string ReplaceNo(this string input)
    {
        string result = input;
        if (!string.IsNullOrEmpty(input))
        {
            for (int i = 0; i < NumArr.Length; i++)
            {
                result = result.Replace(i + "", NumArr[i] + "");
            }
        }
        return result;
    }

    public static string PDate(this object dt)
    {
        return PDate(dt, null);
    }
    public static string PDate(this object dt, string format)
    {
        return dt != null ? PDate((DateTime)dt, format) : string.Empty;
    }
    public static string PDate(this DateTime dt)
    {
        return PDate(dt, null);
    }

    public static string PDate(this DateTime? dt)
    {
        return PDate(dt, null);
    }

    public static string PDate(this DateTime dt, string format)
    {
        if (string.IsNullOrEmpty(format))
            format = "$d $MMMM $HH:$mm";

        return (new VDate(dt)).SetPersianNumber(false).ToString(format);
    }

    public static string ReverseDate(this string input)
    {
        var matchData = Regex.Match(input, "(\\d{4})/(\\d{2})/(\\d{2})");
        if (matchData.Success)
        {
            return string.Format("{2}/{1}/{0}", matchData.Groups[1], matchData.Groups[2], matchData.Groups[3]);
        }

        return input;
    }

    public static string ReverseDate(this int? input)
    {
        if(!input.HasValue)
            return string.Empty;

        var dInput = input.Value.AsPersianDateString();
        var matchData = Regex.Match(dInput, "(\\d{4})/(\\d{2})/(\\d{2})");
        if (matchData.Success)
        {
            return string.Format("{2}/{1}/{0}", matchData.Groups[1], matchData.Groups[2], matchData.Groups[3]);
        }
        return dInput;
    }

    public static string PDate(this DateTime? dt, string format)
    {
        if (!dt.HasValue)
            return string.Empty;

        return PDate(dt.Value, format);
    }
    public static VDate ConvertToShamsi(this string txt)
    {
        VDate dt = null;
        if (!string.IsNullOrEmpty(txt) && VDate.TryParse(txt, out dt))
        {
            return dt;
        }
        return null;
    }
    public static DateTime? ConvertShamsiToDate(this string txt)
    {
        VDate dt = null;
        if (!string.IsNullOrEmpty(txt) && VDate.TryParse(txt, out dt))
        {
            return dt.ToDateTime();
        }
        return null;
    }

    public static byte? AsByte(this string input)
    {
        byte t;
        if (byte.TryParse(input, out t))
            return t;
        return null;
    }

    public static byte AsByte(this object input, byte defaultValue = 0)
    {
        return input == null ? defaultValue : Convert.ToByte(input);
    }

    public static byte? AsByte(this object input, byte? defaultValue)
    {
        var val = defaultValue;
        try
        {
            if (input != null)
                val = Convert.ToByte(input);
        }
        catch (Exception){}

        return val;
    }
    public static string AsAmakenMobileFormat(this string mobileNo)
    {
        if (mobileNo.IsNull())
            return mobileNo;

        if (mobileNo.Length == 11)
            return "+98" + mobileNo.Substring(1);

        return mobileNo;
    }

    public static bool? AsBoolean(this object input, bool? defaultValue)
    {
        var val = defaultValue;
        try
        {
            if (input != null)
                val = Convert.ToBoolean(input);
        }
        catch (Exception) { }

        return val;
    }

    public static long? AsLong(this string input)
    {
        long t;
        if (long.TryParse(input, out t))
            return t;
        return null;
    }
    /// <summary>
    /// Unsafe cast object
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static long AsLong(this object input)
    {
        return Convert.ToInt64(input);
    }

    /// <summary>
    /// Safe Cast
    /// </summary>
    /// <param name="input"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static long? AsLong(this object input, long? defaultValue)
    {
        var val = defaultValue;
        try
        {
            if (input != null)
                val = Convert.ToInt64(input);
        }
        catch (Exception) { }

        return val;
    }

    public static int? AsInt(this object input, int? defaultValue)
    {
        int val;
        try
        {
            if(input == null)
                return defaultValue;

            if (input is int)
                return Convert.ToInt32(input);

            if (int.TryParse(input.ToString(), out val))
                return val;

        }
        catch (Exception) { }

        return defaultValue;
    }

    public static string GetDomainData(this Uri input)
    {
        return string.Format("{0}{1}{2}{3}", input.Scheme, System.Uri.SchemeDelimiter, input.Host,
            (input.IsDefaultPort ? "" : ":" + input.Port));
    }

    public static string GetStr(this byte[] input)
    {
        if (input == null || input.Length <= 0)
            return null;

        return Encoding.UTF8.GetString(input);
    }

    public static int? AsInt(this string input)
    {
        if (input.IsNull())
            return null;

        int t;
        if (int.TryParse(input, out t))
            return t;
        return null;
    }

    public static int Val(this int? input, int defaultValue = 0)
    {
        return input.GetValueOrDefault(defaultValue);
    }

    public static bool Val(this bool? input, bool defaultValue = false)
    {
        return input.GetValueOrDefault(defaultValue);
    }

    public static short Val(this short? input, short defaultValue = 0)
    {
        return input.GetValueOrDefault(defaultValue);
    }

    public static float Val(this float? input, short defaultValue = 0)
    {
        return input.GetValueOrDefault(defaultValue);
    }

    public static long Val(this long? input, long defaultValue = 0)
    {
        return input.GetValueOrDefault(defaultValue);
    }

    public static byte Val(this byte? input, byte defaultValue = 0)
    {
        return input.GetValueOrDefault(defaultValue);
    }

    public static int? AsDateInt(this DateTime input)
    {
        return AsDateInt((DateTime?)input);
    }

    public static VDate AsPersianDate(this int? persianDateInt)
    {
        if (!persianDateInt.HasValue)
            return null;

        var str = persianDateInt.ToString();
        if (string.IsNullOrEmpty(str) || str.Length != 8)
            return null;

        return new VDate(
            Convert.ToInt32(str.Substring(0, 4)),
            Convert.ToInt32(str.Substring(4, 2)),
            Convert.ToInt32(str.Substring(6, 2))
            );
    }

    public static string AsPersianDateString(this int? persianDateInt, string format = "$yyyy/$MM/$dd")
    {
        if (!persianDateInt.HasValue)
            return string.Empty;

        return AsPersianDateString(persianDateInt.Value);
    }

    public static string AsPersianDateString(this int persianDateInt, string format = "$yyyy/$MM/$dd")
    {
        var res = AsPersianDate(persianDateInt);
        return res != null ? res.ToString(format) : string.Empty;
    }

    public static string AsString(this DateTime? input, string format = "yyyy/MM/dd")
    {
        return !input.HasValue ? string.Empty : input.Value.ToString(format);
    }

    public static DateTime? AsDate(this int? persianDate)
    {
        if (!persianDate.HasValue)
            return null;

        var str = persianDate.ToString();
        if (string.IsNullOrEmpty(str) || str.Length != 8)
            return null;

        var pDate = new VDate(
            Convert.ToInt32(str.Substring(0,4)),
            Convert.ToInt32(str.Substring(4,2)),
            Convert.ToInt32(str.Substring(6,2))
            );

        return pDate.ToDateTime();
    }

    public static DateTime AsDate(this int persianDate)
    {
        var dateTime = AsDate((int?) persianDate);
        if (dateTime == null)
            throw new Exception("اشکال در تبدیل تاریخ");

        return dateTime.Value;
    }

    private static readonly string[] _sabtGender = {"زن", "مرد"};
    public static string SabetAhvalGender(this int? sabtGender)
    {
        return _sabtGender[sabtGender.Val()];
    }

    private static readonly string[] _liveStatus = {"زنده", "مرده"};

    public static string SabteAhvalLiveStatus(this int? sabtLiveStatus)
    {
        return _liveStatus[sabtLiveStatus.Val()];
    }

    public static int? AsDateInt(this DateTime? input)
    {
        if (!input.HasValue) return null;

        VDate dt = new VDate((DateTime) input);
        return (int) dt;
    }

    public static int AsInt(this object input)
    {
        if (input == null)
            return 0;

        try
        {
            return Convert.ToInt32(input);
        }
        catch (Exception)
        {
            return 0;
        }
    }

    public static string FormatIntDate(this int? date)
    {
        if (!date.HasValue)
            return string.Empty;
        var dateStr = date.ToString();
        return string.Format("{0}/{1}/{2}", dateStr.Substring(0, 4), dateStr.Substring(4, 2), dateStr.Substring(6, 2));
    }

    public static bool IsIran(this int? input, bool includeNullAsIran = true, int iranCode = 18)
    {
        if (includeNullAsIran)
            return input.Val(iranCode) == iranCode;

        return input.Val() == iranCode;
    }

    public static bool IsIran(this int input, bool zeroIsIran = true, int iranCode = 18)
    {
        if (zeroIsIran && input == 0)
            return true;

        return input == iranCode;
    }

    public static short? AsShort(this string input)
    {
        short t;
        if (short.TryParse(input, out t))
            return t;
        return null;
    }

    public static short AsShort(this object input)
    {
        return Convert.ToInt16(input);
    }

    public static short? AsShort(this object input, short? defaultValue)
    {
        var val = defaultValue;
        try
        {
            if (input != null)
                val = Convert.ToInt16(input);
        }
        catch (Exception) { }

        return val;
    }

    public static float? AsSingle(this object input)
    {
        if (input == null)
            return null;

        return Convert.ToSingle(input);
    }

    public static float? AsSingle(this object input, float? defaultValue)
    {
        var val = defaultValue;
        try
        {
            if (input != null)
                val = Convert.ToSingle(input);
        }
        catch (Exception) { }

        return val;
    }

    public static string AsNullVal(this string input)
    {
        return (string.IsNullOrEmpty(input) ? null : input);
    }

    public static bool IsNull(this string input)
    {
        return string.IsNullOrEmpty(input);
    }

    public static bool IsNullCheck(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return true;

        if (string.IsNullOrEmpty(input.Trim()))
            return true;

        if (input.Trim() == "0")
            return true;

        return false;
    }

    public static string PersianCorrection(this string str)
    {
        //ی U+FEF1 Arabic Letter Yeh Isolated Form
        //ی U+064A Arabic Letter Yeh
        //ى U+0649 Arabic Letter Alef Maksura
        //ی U+06CC Arabic Letter Farsi Yeh

        //ك U+0643 Arabic Letter Kaf
        //ک U+069A Arabic Letter Keh

        //U+0660 	٠ 	Arabic-Indic Digit Zero
        //U+0661 	١ 	Arabic-Indic Digit One
        //U+0662 	٢ 	Arabic-Indic Digit Two
        //U+0663 	٣ 	Arabic-Indic Digit Three
        //U+0664 	٤ 	Arabic-Indic Digit Four
        //U+0665 	٥ 	Arabic-Indic Digit Five
        //U+0666 	٦ 	Arabic-Indic Digit Six
        //U+0667 	٧ 	Arabic-Indic Digit Seven
        //U+0668 	٨ 	Arabic-Indic Digit Eight
        //U+0669 	٩ 	Arabic-Indic Digit Nine

        //U+06F0 	۰ 	Extended Arabic-Indic Digit Zero
        //U+06F1 	۱ 	Extended Arabic-Indic Digit One
        //U+06F2 	۲ 	Extended Arabic-Indic Digit Two
        //U+06F3 	۳ 	Extended Arabic-Indic Digit Three
        //U+06F4 	۴ 	Extended Arabic-Indic Digit Four
        //U+06F5 	۵ 	Extended Arabic-Indic Digit Five
        //U+06F6 	۶ 	Extended Arabic-Indic Digit Six
        //U+06F7 	۷ 	Extended Arabic-Indic Digit Seven
        //U+06F8 	۸ 	Extended Arabic-Indic Digit Eight
        //U+06F9 	۹ 	Extended Arabic-Indic Digit Nine


        //return str.Replace('ی', 'ی').Replace('ی', 'ی').Replace('ى', 'ی').Replace('ﻳ', 'ی').Replace('ﻱ', 'ی').Replace('ﻲ', 'ی').Replace('ﻰ', 'ی').Replace('ﻯ', 'ی').Replace('ك', 'ک')
        //    .Replace('٠', '۰').Replace('١', '۱').Replace('٢', '۲').Replace('٣', '۳').Replace('٤', '۴')
        //    .Replace('٥', '۵').Replace('٦', '۶').Replace('٧', '۷').Replace('٨', '۸').Replace('٩', '۹');
        if (string.IsNullOrEmpty(str))
            return str;

        const char persianYeh = '\u06cc';
        return str.Replace('\u064a', persianYeh) //Arabic Letter Yeh
            .Replace('\u0649', persianYeh) //Arabic Letter Alef Maksura
            .Replace('\ufbfc', persianYeh) //Arabic Letter Yeh Isolated Form
            .Replace('\ufeef', persianYeh) //Arabic Letter Alef Maksura Isolated Form
            .Replace('\ufef1', persianYeh) //Arabic Letter Yeh Isolated Form
            .Replace('\ufb8e', '\u06a9') //Arabic Letter Keheh Isolated Form
            .Replace('\u0643', '\u06a9')   //Arabic Letter Kaf, Farsi Keheh

            //numbers
            .Replace('\u0660', '0')   //0
            .Replace('\u0661', '1')   //1
            .Replace('\u0662', '2')   //2
            .Replace('\u0663', '3')   //3
            .Replace('\u0664', '4')   //4
            .Replace('\u0665', '5')   //5
            .Replace('\u0666', '6')   //6
            .Replace('\u0667', '7')   //7
            .Replace('\u0668', '8')   //8
            .Replace('\u0669', '9')   //9

            .Replace('\u06f0', '0')   //0
            .Replace('\u06f1', '1')   //1
            .Replace('\u06f2', '2')   //2
            .Replace('\u06f3', '3')   //3
            .Replace('\u06f4', '4')   //4
            .Replace('\u06f5', '5')   //5
            .Replace('\u06f6', '6')   //6
            .Replace('\u06f7', '7')   //7
            .Replace('\u06f8', '8')   //8
            .Replace('\u06f9', '9');  //9

    }

    public static string AsRtf(this object input, string nullValue = "", string fontIndex = "f0")
    {
        if (input == null || input == DBNull.Value || string.IsNullOrEmpty(input.ToString()))
        {
            return string.IsNullOrEmpty(nullValue)
                ? string.Empty
                : string.Format("\\{0} {1}", fontIndex, CheckChar(nullValue));
        }

        return string.Format("\\{0} {1}", fontIndex, CheckChar(input.ToString()));
    }

    private static readonly char[] Slashable = new[] { '{', '}', '\\' };
    public static string CheckChar(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            if (value.IndexOfAny(Slashable) >= 0)
            {
                value = value.Replace("{", "\\{").Replace("}", "\\}").Replace("\\", "\\\\");
            }
            bool replaceuni = false;
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] > 255)
                {
                    replaceuni = true;
                    break;
                }
            }
            if (replaceuni)
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < value.Length; i++)
                {
                    if (value[i] <= 255)
                    {
                        sb.Append(value[i]);
                    }
                    else
                    {
                        sb.Append("\\u");
                        sb.Append((int)value[i]);
                        sb.Append("?");
                    }
                }
                value = sb.ToString();
            }
        }


        return value;
    }

    public static byte? ZeroAsNull(this byte? input)
    {
        if (input.HasValue && input.Value <= 0)
            return null;

        return input;
    }

    public static int? ZeroAsNull(this int? input)
    {
        if (input.HasValue && input.Value <= 0)
            return null;

        return input;
    }

    public static long? ZeroAsNull(this long? input)
    {
        if (input.HasValue && input.Value <= 0)
            return null;

        return input;
    }

    public static float? ZeroAsNull(this float? input)
    {
        if (input.HasValue && input.Value <= 0)
            return null;

        return input;
    }

    public static string ConvertLines(this object input)
    {
        if (input == null)
            return "";

        return (input as string).Replace("\r\n", "<br />")
            .Replace("\r", "<br />").Replace("\n", "<br />");
    }
}

