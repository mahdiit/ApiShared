using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ExcelHelper
{
    public class ExcelListImporter : IDisposable
    {
        private Stream stream;
        private ExcelWorksheet currentWorksheet;
        private ExcelPackage package;
        private ExcelWorkbook workbook;

        public Exception LoadFileException { get; set; }

        public ExcelListImporter SetStream(Stream excelStream)
        {
            stream = excelStream;
            return this;
        }

        public bool LoadFile()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            try
            {
                package = new ExcelPackage(stream);
                workbook = package.Workbook;
                if (workbook == null)
                {
                    LoadFileException = new Exception("Workbook not found");
                    return false;
                }
            }
            catch (Exception ex)
            {
                LoadFileException = ex;
                return false;
            }

            return true;
        }

        public bool SetCurrentSheet(string sheetName)
        {
            if (workbook.Worksheets.Any(c => c.Name == sheetName))
            {
                currentWorksheet = workbook.Worksheets[sheetName];
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<T> GetList<T>() where T : new()
        {
            List<T> collection = new List<T>();
            try
            {
                DataTable dt = new DataTable();
                foreach (var firstRowCell in new T().GetType().GetProperties().ToList())
                {
                    //Add table colums with properties of T
                    dt.Columns.Add(firstRowCell.Name);
                }
                for (int rowNum = 2; rowNum <= currentWorksheet.Dimension.End.Row; rowNum++)
                {
                    var wsRow = currentWorksheet.Cells[rowNum, 1, rowNum, currentWorksheet.Dimension.End.Column];
                    DataRow row = dt.Rows.Add();
                    foreach (var cell in wsRow)
                    {
                        row[cell.Start.Column - 1] = cell.Text;
                    }
                }

                //Get the colums of table
                var columnNames = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();

                //Get the properties of T
                List<PropertyInfo> properties = new T().GetType().GetProperties().ToList();

                collection = dt.AsEnumerable().Select(row =>
                {
                    T item = Activator.CreateInstance<T>();
                    foreach (var pro in properties)
                    {
                        if (columnNames.Contains(pro.Name) || columnNames.Contains(pro.Name.ToUpper()))
                        {
                            PropertyInfo pI = item.GetType().GetProperty(pro.Name);
                            pro.SetValue(item, (row[pro.Name] == DBNull.Value) ? null : Convert.ChangeType(row[pro.Name], (Nullable.GetUnderlyingType(pI.PropertyType) == null) ? pI.PropertyType : Type.GetType(pI.PropertyType.GenericTypeArguments[0].FullName)));
                        }
                    }
                    return item;
                }).ToList();

            }
            catch (Exception ex)
            {
                LoadFileException = ex;
            }

            return collection;
        }

        public void Dispose()
        {
            if (stream != null)
                stream.Dispose();

            if (package != null)
                package.Dispose();
        }
    }
}
