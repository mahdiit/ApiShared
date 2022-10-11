using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelExporter
{
    public static class ExportHelper
    {
        public static Task<FileContentResult> ToExcel(this byte[] data, ControllerBase controller, string fileName)
        {
            return Task.FromResult(controller.File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName + ".xlsx"));
        }

        public static Task<FileContentResult> ToText(this string data, ControllerBase controller)
        {
            var byteArr = Encoding.UTF8.GetBytes(data);
            return Task.FromResult(controller.File(byteArr, "text/plain", "empty.txt"));
        }
    }
}
