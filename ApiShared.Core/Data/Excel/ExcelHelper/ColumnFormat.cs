using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiShared.Core.Data.Excel.ExcelHelper
{
    public sealed class ColumnsFormat
    {
        public const string OneDecimalPlace = "0.0";
        public const string TwoDecimalPlace = "0.00";
        public const string TwoDecimalPlaceThousandSeparator = "#,##0.00";
        public const string Percent = "0%";
        public const string ThousandSeparator = "#,##0";
        public const string NegativeRed = "#,##0.00_);[Red](#,##0.00)";
    }
}
