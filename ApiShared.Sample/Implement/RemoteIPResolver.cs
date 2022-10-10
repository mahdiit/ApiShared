using ApiShared.Core.Middlewares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiShared.Sample.Implement
{
    public class RemoteIPResolver : IRemoteIPResolver
    {
        public string? CallerIP
        {
            get
            {
                return "Console App";
            }
        }
    }
}
