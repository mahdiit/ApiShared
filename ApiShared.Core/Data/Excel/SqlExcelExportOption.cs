using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiShared.Core.Data.Excel
{
    public class SqlExcelExportOption
    {
        public SqlExcelExportOption(List<string> sheetNames)
        {
            SheetNames = sheetNames;
            Rtl = true;
            HasRowNumber = true;
            AutoFitColumns = true;
        }        
        public string? FileName { get; set; }
        public Dictionary<string, string>? ColumnNames { get; set; }
        public List<string> SheetNames { get; set; }
        public List<string>? SheetTitles { get; set; }
        public Dictionary<string, string>? Conditions { get; set; }
        public bool Rtl { get; set; }
        public bool HasRowNumber { get; set; }
        public bool AutoFitColumns { get; set; }
        public RenderColumn? OnRenderColumn { get; set; } = null;
    }
}
