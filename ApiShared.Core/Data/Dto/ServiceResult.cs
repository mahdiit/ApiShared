using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiShared.Core.Data.Dto
{
    public class ServiceResult
    {
        public int ErrorCode { get; set; } = 0;
        public string? Message { get; set; }
    }

    public class ServiceResult<T>: ServiceResult
    {
        public long DataCount { get; set; } = 0;
        public T? Data { get; set; }
    }
}
