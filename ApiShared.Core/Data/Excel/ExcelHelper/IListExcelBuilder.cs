using System.Collections.Generic;

namespace ApiShared.Core.Data.Excel.ExcelHelper
{
    public interface IListExcelBuilder
    {
        RenderColumn? OnRenderColumn { get; set; }

        ListExcelBuilder AddSheet<T>(string name, List<T> data, bool autoFitColumns);
        byte[] Build();
        ListExcelBuilder Create(bool addDefaultStyle);
        void Dispose();
    }
}