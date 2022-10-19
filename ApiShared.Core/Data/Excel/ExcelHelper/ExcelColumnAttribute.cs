using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExcelExporter
{
    public class ExcelColumnAttribute : Attribute
    {
        /// <summary>
        /// نام ستون در خروجی
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// آیا به دنبال مقدار در دیتاها بگردد
        /// </summary>
        public bool HasValue { get; set; } = true;

        /// <summary>
        /// تاریخ به شمسی تبدیل شود فقط در مورد تاریخ اعمال میشود
        /// </summary>
        public bool ConvertToPersianDate { get; set; }

        /// <summary>
        /// فرمت تاریخ فقط در مورد تاریخ اعمال میشود
        /// </summary>
        public string? DateFormat { get; set; }

        /// <summary>
        /// فرمت خروجی ستون در اکسل
        /// </summary>
        public string? ColumnFormat { get; set; }

        /// <summary>
        /// نام ستون در دادهها
        /// </summary>
        public string? SourceName { get; set; }
    }
}
