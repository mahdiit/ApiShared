using System.Collections.Generic;

namespace ExcelExporter
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