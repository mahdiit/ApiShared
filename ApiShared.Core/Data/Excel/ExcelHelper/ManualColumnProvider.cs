using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelExporter
{
    public class ManualColumnProvider : IColumnProvider
    {
        private readonly List<ExcelColumnAttribute> _cols;
        public ManualColumnProvider(List<ExcelColumnAttribute> columns)
        {
            _cols = columns;
        }

        public List<ExcelColumnAttribute> GetColumns<T>()
        {
            return _cols;
        }
    }
}
