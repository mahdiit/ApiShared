using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiShared.Core.Data.BaseInterface
{
    /// <summary>
    /// موجودیت دارای سال مالی هست: FiscalYearId
    /// </summary>
    public class IEntityHasFiscalYear
    {
        public int FiscalYearId { get; set; }
    }
}
