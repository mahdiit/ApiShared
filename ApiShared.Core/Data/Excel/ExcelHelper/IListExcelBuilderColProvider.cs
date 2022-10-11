using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelExporter
{
    public interface IColumnProvider
    {
        List<ExcelColumnAttribute> GetColumns<T>();
    }
}
