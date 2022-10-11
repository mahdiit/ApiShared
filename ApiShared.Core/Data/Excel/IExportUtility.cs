using ApiShared.Core.Data.Dto;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiShared.Core.Data.Excel
{
    public delegate void RenderColumn(string column, object value, ExcelRange excelColumn);
    public interface ISqlExcelExport
    {   
        ServiceResult<byte[]> GetFile<T>(SqlQueryDto sqlQuery, SqlExcelExportOption? option = null);

    }
}
