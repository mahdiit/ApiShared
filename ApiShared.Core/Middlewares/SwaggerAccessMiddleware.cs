using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiShared.Core.Middlewares
{
    /// <summary>
    /// دسترسی به سواگر
    /// </summary>
    public class SwaggerAccessMiddleware
    {
        private readonly RequestDelegate _next;

        public SwaggerAccessMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                if (!System.Net.IPAddress.IsLoopback(context.Connection.RemoteIpAddress))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
            }

            await _next.Invoke(context);
        }
    }
}
