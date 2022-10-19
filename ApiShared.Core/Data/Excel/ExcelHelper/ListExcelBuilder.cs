using ApiShared.Core.Dates;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Style.XmlAccess;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ExcelExporter
{
    /// <summary>
    /// در صورتی که ستون رند شد صحیح و در غیر این صورت False
    /// </summary>
    /// <param name="column"></param>
    /// <param name="value"></param>
    /// <param name="excelColumn"></param>
    /// <param name="rowData"></param>
    /// <returns></returns>
    public delegate bool RenderColumn(string column, object? value, ExcelRange excelColumn, Dictionary<string, object?> rowData);
    public delegate void CreateWorksheet(ExcelWorksheet ws);
    public delegate void CreateColumn(ExcelColumn column);

    public class ListExcelBuilder : IDisposable, IListExcelBuilder
    {
        private MemoryStream? memoryStream;
        private ExcelPackage? xlsx;

        /// <summary>
        /// در صورت نیاز به تغییر در هنگام خروجی گرفتن استفاده شود
        /// </summary>
        public RenderColumn? OnRenderColumn { get; set; }

        /// <summary>
        /// تغییر تنظیمات
        /// </summary>
        public CreateWorksheet? OnCreateWorksheet { get; set; }

        /// <summary>
        /// تغییر ستونها
        /// </summary>
        public CreateColumn? OnCreateColumn { get; set; }

        IColumnProvider columnProvider;
        IValueProvider valueProvider;

        public ListExcelBuilder()
        {
            columnProvider = new DefaultColumnProvider();
            valueProvider = new DefaultValueProvider();
        }

        /// <summary>
        /// درصورتی که نام ستونها باید تغییر کند قبل از AddSheet فراخوانی شود
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public ListExcelBuilder SetColumnProvider(IColumnProvider provider)
        {
            columnProvider = provider;
            return this;
        }

        public ListExcelBuilder SetValueProvider(IValueProvider provider)
        {
            valueProvider = provider;
            return this;
        }

        public ListExcelBuilder Create(bool setDefaultStyle = true)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            if (xlsx == null)
            {
                memoryStream = new MemoryStream();
                xlsx = new ExcelPackage(memoryStream);

                if (setDefaultStyle)
                {
                    var headerStyle = CreateStyle("CmHeaderStyle");
                    HeaderStyleName = "CmHeaderStyle";
                    headerStyle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    headerStyle.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    headerStyle.Style.WrapText = false;
                    headerStyle.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    headerStyle.Style.Fill.BackgroundColor.SetColor(Color.Gainsboro);

                    var cellStyle = CreateStyle("CmCellStyle");
                    CellStyleName = "CmCellStyle";
                    cellStyle.Style.Font.Name = "Tahoma";
                    cellStyle.Style.Font.Size = 9.5f;
                }
            }
            return this;
        }

        public string? CellStyleName { get; set; }
        public string? HeaderStyleName { get; set; }
        public bool IsRTL { get; set; } = true;

        public ExcelNamedStyleXml CreateStyle(string name)
        {
            if (xlsx == null)
                throw new Exception("document is empty");

            return xlsx.Workbook.Styles.CreateNamedStyle(name);
        }

        public ListExcelBuilder AddSheet<T>(string name, List<T> data, bool autoFitColumns = true)
        {
            if (xlsx == null)
                throw new Exception("document is empty");

            var colInfoList = columnProvider.GetColumns<T>();

            var ws = xlsx.Workbook.Worksheets.Add(name);

            if (OnCreateWorksheet != null)
                OnCreateWorksheet(ws);

            if (!string.IsNullOrEmpty(CellStyleName))
                ws.Cells.StyleName = CellStyleName;

            if (!string.IsNullOrEmpty(HeaderStyleName))
                ws.Cells[1, 1].StyleName = HeaderStyleName;

            ws.Cells[1, 1].Value = "ردیف";

            int totalCount = 0;

            //Write Headers
            for (int i = 0; i < colInfoList.Count; i++)
            {
                if (!string.IsNullOrEmpty(HeaderStyleName))
                    ws.Cells[1, i + 2].StyleName = HeaderStyleName;

                ws.Cells[1, i + 2].Value = colInfoList[i].Name;

                if (OnCreateColumn != null)
                    OnCreateColumn(ws.Column(i + 2));
            }

            var sourceColumns = colInfoList.Where(c => c.HasValue && c.SourceName != null).Select(c => c.SourceName).ToArray();
            foreach (var row in data)
            {
                totalCount++;

                if (row == null)
                    continue;

                ws.Cells[totalCount + 1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[totalCount + 1, 1].Value = totalCount;

                var colsData = valueProvider.GetValues(sourceColumns, row);
                var colsIndex = 0;
                foreach (var colData in colsData)
                {
                    var col = ws.Cells[totalCount + 1, colsIndex + 2];

                    bool isRenderFinish = false;
                    if (OnRenderColumn != null)
                    {
                        var objVal = colInfoList[colsIndex].HasValue ? colData.Value : null;
                        isRenderFinish = OnRenderColumn(colData.Key, objVal, col, colsData);
                    }

                    if (!isRenderFinish && colInfoList[colsIndex].HasValue)
                    {
                        col.Value = colData.Value;
                        var columnFormat = colInfoList[colsIndex].ColumnFormat;
                        var dateFormat = colInfoList[colsIndex].DateFormat;

                        if (colInfoList[colsIndex].ConvertToPersianDate && colData.Value is DateTime)
                        {
                            var vDate = new VDate((DateTime)colData.Value);
                            if (dateFormat == null)
                            {
                                col.Value = vDate.ToString("$yyyy/$MM/$dd");
                            }
                            else
                            {
                                col.Value = vDate.ToString(dateFormat);
                            }
                        }
                        else if (columnFormat == null && colData.Value is DateTime)
                        {
                            col.Style.Numberformat.Format = "yyyy/MM/dd HH:mm:ss";
                        }
                        else if (columnFormat != null)
                            col.Style.Numberformat.Format = colInfoList[colsIndex].ColumnFormat;
                    }

                    colsIndex++;
                }

                //بیش اندازه بزرگ شده است
                if (totalCount == 1000000)
                {
                    break;
                }
            }

            if (autoFitColumns)
                ws.Cells.AutoFitColumns();

            ws.View.PageLayoutView = false;
            ws.View.RightToLeft = IsRTL;
            return this;
        }

        public byte[] Build()
        {
            if (xlsx == null || memoryStream == null)
                throw new Exception("document is empty");

            xlsx.Save();
            return memoryStream.ToArray();
        }

        public void Dispose()
        {
            if (xlsx != null)
                xlsx.Dispose();

            if (memoryStream != null)
                memoryStream.Dispose();
        }
    }
}
