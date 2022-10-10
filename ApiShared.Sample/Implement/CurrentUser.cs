using ApiShared.Core.Data.BaseInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiShared.Sample.Implement
{
    public class CurrentUser : ICurrentUser
    {
        public bool IsSuperAdmin { get { return false; } }

        public string Path => "/1/2/3/";

        public int? CompanyId => 2;

        public int? BranchId => 1;

        public int? FiscalYearId => 3;

        public string UserName => "m.yousefi";

        public int UserId => 10;
    }
}
