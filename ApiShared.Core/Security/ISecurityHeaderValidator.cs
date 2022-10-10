using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiShared.Core.Security
{
    public interface ISecurityHeaderValidator
    {
        public bool IsValid(string headerValue, out Dictionary<string, string> headers);
    }
}
