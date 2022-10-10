using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiShared.Core.Middlewares
{
    public interface IRemoteIPResolver
    {
        /// <summary>
        /// اطلاعات آی پی فرد متصل شونده
        /// </summary>
        public string? CallerIP { get; }
    }
}
