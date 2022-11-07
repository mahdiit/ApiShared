using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text;

namespace ApiShared.Core.Middlewares
{
    //https://rimdev.io/middleware-madness-site-maintenance-in-aspnet-core/
    public class MaintenanceMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger logger;
        private readonly MaintenanceWindow window;

        public MaintenanceMiddleware(RequestDelegate next, MaintenanceWindow window, ILogger<MaintenanceMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
            this.window = window;
        }

        public async Task Invoke(HttpContext context)
        {
            if (window.Enabled)
            {
                // set the code to 503 for SEO reasons
                context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                context.Response.Headers.Add("Retry-After", window.RetryAfterInSeconds.ToString());
                context.Response.ContentType = window.ContentType;
                await context
                    .Response
                    .WriteAsync(Encoding.UTF8.GetString(window.Response), Encoding.UTF8);
            }
            await next.Invoke(context);
        }
    }
}