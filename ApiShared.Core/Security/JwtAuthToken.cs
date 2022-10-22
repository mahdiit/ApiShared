using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiShared.Core.Security
{
    public class JwtAuthToken
    {
        public string Token { get; set; }
        public long Expires { get; set; }
    }
}
