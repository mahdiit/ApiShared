using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiShared.Core.Data.BaseClass
{
    public class AppException: Exception
    {
        public int? ErrorCode { get; set; }
        public object? MetaData { get; set; }

        public AppException()
        {

        }

        public AppException(string message): base(message)
        {

        }
    }
}
