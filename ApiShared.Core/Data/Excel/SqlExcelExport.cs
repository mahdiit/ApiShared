using ApiShared.Core.Data.Dto;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiShared.Core.Data.Excel
{
    public class SqlExcelExport : ISqlExcelExport
    {
        private readonly ILogger Log;
        public SqlExcelExport(ILogger<SqlExcelExport> logger)
        {
            Log = logger;
        }

        public ServiceResult<byte[]> GetFile<T>(SqlQueryDto sqlQuery, SqlExcelExportOption? option = null)
        {
            var result = new ServiceResult<byte[]>();

            var sqlConnection = new SqlConnection(sqlQuery.ConnectionString);
            var sqlCommand = new SqlCommand(sqlQuery.SqlCommand, sqlConnection)
            {
                CommandTimeout = sqlQuery.CommandTimeout
            };

            if (sqlQuery.CommandParameter != null)
                foreach (string key in sqlQuery.CommandParameter.Keys)
                {
                    sqlCommand.Parameters.AddWithValue(key, sqlQuery.CommandParameter[key] ?? DBNull.Value);
                }

            sqlConnection.Open();
            SqlDataReader? reader = null;
            MemoryStream? memoryStream = null;

            if (option == null)
                option = new SqlExcelExportOption(new List<string>() { });

            try
            {
                ExcelPackage xlsx;
                if (option.FileName != null)
                    xlsx = new ExcelPackage(new FileInfo(option.FileName));
                else
                {
                    memoryStream = new MemoryStream();
                    xlsx = new ExcelPackage(memoryStream);
                }

                reader = sqlCommand.ExecuteReader();
                int sheetIndex = 0;
                do
                {
                    int totalCount = 0;
                    bool hasHeader = false;

                    if (option.ColumnNames == null)
                    {
                        option.ColumnNames = new Dictionary<string, string>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            var colName = reader.GetName(i);
                            option.ColumnNames.Add(colName, colName);
                        }
                    }

                    var colPos = new Dictionary<string, int>();
                    ExcelWorksheet ws;
                    if (option.SheetNames.Count >= (sheetIndex + 1))
                        ws = xlsx.Workbook.Worksheets.Add(option.SheetNames[sheetIndex]);
                    else
                        ws = xlsx.Workbook.Worksheets.Add("Result-" + sheetIndex);

                    ws.Cells.Style.Font.Name = "Tahoma";
                    ws.Cells.Style.Font.Size = 9.5f;
                    ws.Cells.Style.Numberformat.Format = "@";

                    if (option.SheetTitles != null && option.SheetTitles.Count >= (sheetIndex + 1))
                    {
                        hasHeader = true;
                        var firstHeader = ws.Cells[1, 1, 1, option.ColumnNames.Count + 1];
                        firstHeader.Merge = true;
                        firstHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        firstHeader.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        firstHeader.Style.Font.Bold = true;
                        firstHeader.Value = option.SheetTitles[sheetIndex];
                        ws.Row(1).Height = 25;
                    }

                    if (option.HasRowNumber)
                    {
                        int cellName = hasHeader ? 2 : 1;
                        ws.Cells[cellName, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[cellName, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws.Cells[cellName, 1].Style.WrapText = false;
                        ws.Cells[cellName, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[cellName, 1].Style.Fill.BackgroundColor.SetColor(Color.Gainsboro);
                        ws.Cells[cellName, 1].Value = option.Rtl ? "دریف" : "No";
                    }

                    while (reader.Read())
                    {
                        if (totalCount == 0)
                        {
                            //پیدا کردن شماره ستونها در خروجی
                            int i;
                            for (i = 0; i < reader.FieldCount; i++)
                            {
                                var colName = reader.GetName(i);
                                if (option.ColumnNames.ContainsKey(colName))
                                    colPos.Add(colName, i);
                            }

                            i = 1;
                            int rowNo = hasHeader ? 2 : 1;
                            foreach (var item in option.ColumnNames)
                            {

                                int cellNo = option.HasRowNumber ? i + 1 : i;

                                ws.Cells[rowNo, cellNo].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                ws.Cells[rowNo, cellNo].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                ws.Cells[rowNo, cellNo].Style.WrapText = false;
                                ws.Cells[rowNo, cellNo].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                ws.Cells[rowNo, cellNo].Style.Fill.BackgroundColor.SetColor(Color.Gainsboro);
                                ws.Cells[rowNo, cellNo].Value = item.Value;
                                i++;
                            }

                            ws.Row(rowNo).Height = 20;
                            ws.Cells.AutoFitColumns();
                        }

                        totalCount++;

                        ws.Row(hasHeader ? totalCount + 1 : totalCount).Height = 16.5;

                        if (option.HasRowNumber)
                        {
                            ws.Cells[hasHeader ? totalCount + 2 : totalCount + 1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells[hasHeader ? totalCount + 2 : totalCount + 1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells[hasHeader ? totalCount + 2 : totalCount + 1, 1].Value = totalCount;
                        }

                        var j = option.HasRowNumber ? 1 : 0;
                        foreach (var item in option.ColumnNames)
                        {
                            var col = ws.Cells[totalCount + 2, j + 1];
                            col.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                            var colValue = reader.GetValue(colPos[item.Key]);

                            if (option.OnRenderColumn != null)
                                option.OnRenderColumn(item.Key, colValue, col);
                            else
                                col.Value = colValue;

                            j++;
                        }

                        if (totalCount % 100 == 0 && totalCount < 3000)
                        {
                            ws.Cells.AutoFitColumns();
                            //OnProgress("تعداد رکوردهای پردازش شده:  " + totalCount);
                        }

                        //بیش اندازه بزرگ شده است
                        if (totalCount == 1000000)
                        {
                            break;
                        }
                    }

                    ws.View.PageLayoutView = false;

                    if (option.Rtl)
                        ws.View.RightToLeft = true;

                    if (option.Conditions != null && option.Conditions.Count > 0)
                    {
                        var wsCond = xlsx.Workbook.Worksheets.Add("Conditions");
                        wsCond.Cells.Style.Font.Name = "Tahoma";
                        wsCond.Cells.Style.Font.Size = 9.5f;
                        wsCond.Cells.Style.Numberformat.Format = "@";

                        var firstHeader = wsCond.Cells[1, 1, 1, 2];
                        firstHeader.Merge = true;
                        firstHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        firstHeader.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        firstHeader.Style.Font.Bold = true;
                        firstHeader.Value = "شرط های گزارش";
                        wsCond.Row(1).Height = 25;

                        var i = 0;
                        foreach (var condition in option.Conditions)
                        {
                            wsCond.Cells[i + 2, 1].Value = condition.Key;
                            wsCond.Cells[i + 2, 2].Value = condition.Value;
                            wsCond.Row(i + 2).Height = 17;
                            i++;
                        }

                        wsCond.View.PageLayoutView = false;
                        wsCond.View.RightToLeft = true;
                    }



                    sheetIndex++;
                } while (reader.NextResult());

                xlsx.Save();
                result.Message = "با موفقیت ساخته شد";

                if (memoryStream != null)
                    result.Data = memoryStream.ToArray();
                else if (option.FileName != null)
                    result.Data = File.ReadAllBytes(option.FileName);

                result.DataCount = 1;
                xlsx.Dispose();
            }
            catch (Exception ex)
            {
                Log.LogError(ex, "ExportUtility.GetFile", new { Data = sqlQuery, DataType = sqlQuery.GetType() });
                result.ErrorCode = -1;
                result.Message = "خطا در تولید فایل اکسل";
            }
            finally
            {
                if (memoryStream != null)
                    memoryStream.Dispose();

                if (reader != null)
                    reader.Close();

                //clean command
                sqlCommand.Dispose();

                //clean connection
                sqlConnection.Close();
                sqlConnection.Dispose();
            }

            return result;
        }
    }
}
