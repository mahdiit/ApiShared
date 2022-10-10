using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiShared.Core.Data.BaseInterface
{
    public interface ICurrentUser
    {
        public bool IsSuperAdmin { get;  }
        public string Path { get; }
        public int? CompanyId { get; }
        public int? BranchId { get; }
        public int? FiscalYearId { get; }
        public string UserName { get; }
        public int UserId { get; }
    }
}
