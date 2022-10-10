using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ApiShared.Core
{
    public static class StaticParameters
    {
        public static class CoreContext
        {
            public static List<Assembly> ConfigurationsAssembly = new List<Assembly>();
        }

        public static class Security
        {
            public static class ApiHeader
            {
                public const string EncryptKey = "~Wk!93w)#~xD?.'.";
                public const string Name = "X-Security";                
                public const string QueryKey = "XSec";

                public const string Partycode = "X-Partycode";
                public const string Username = "X-Username";
                public const string IPAddress = "X-IPAddress";
                public const string Time = "X-Time";
                public const string Company = "X-Company";
                public const string Branch = "X-Branch";
                public const string FiscalYear = "X-FiscalYear";
                public const string Userid = "X-Userid";
            }
        }
    }
}
