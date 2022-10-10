using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiShared.Core.Middlewares
{
    public class RemoteIPResolver : IRemoteIPResolver
    {
        IHttpContextAccessor RequestContext;
        public RemoteIPResolver(IHttpContextAccessor httpContext)
        {
            RequestContext = httpContext;
        }

        private bool CanNotGetRequest
        {
            get
            {
                return (RequestContext.HttpContext == null || RequestContext.HttpContext.Request == null);
            }
        }

        public string? CallerIP
        {
            get
            {
                try
                {
                    if (CanNotGetRequest)
                        return string.Empty;

                    string ip = string.Empty;

                    var xForwardedFor = RequestContext.HttpContext.Request.Headers["X-Forwarded-For"];
                    var httpXForwardedFor = RequestContext.HttpContext.Request.Headers["HTTP_X_FORWARDED_FOR"];
                    var httpForwarded = RequestContext.HttpContext.Request.Headers["HTTP_FORWARDED"];
                    var httpClientIp = RequestContext.HttpContext.Request.Headers["HTTP_CLIENT_IP"];

                    var remoteAddr = RequestContext.HttpContext.Connection.RemoteIpAddress?.ToString();

                    if (!string.IsNullOrEmpty(xForwardedFor))
                        ip = xForwardedFor;
                    else if (!string.IsNullOrEmpty(httpXForwardedFor))
                        ip = httpXForwardedFor;
                    else if (!string.IsNullOrEmpty(httpClientIp))
                        ip = httpClientIp;
                    else if (!string.IsNullOrEmpty(httpForwarded))
                        ip = httpForwarded;
                    else if (!string.IsNullOrEmpty(remoteAddr))
                        ip = remoteAddr;

                    if (ip == null) return ip;
                    string[] ipRange = ip.Split(',');
                    ip = ipRange[0].Trim();

                    return ip;
                }
                catch (Exception)
                {
                    return "ip-error";
                }
            }
        }
    }
}
