using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ApiShared.Core.Middlewares
{

    public static class MaintenanceWindowExtensions
    {
        public static IServiceCollection AddMaintenance(this IServiceCollection services, MaintenanceWindow window)
        {
            services.AddSingleton(window);
            return services;
        }

        public static IServiceCollection AddMaintenance(this IServiceCollection services, Func<bool> enabler, byte[] response, string contentType = "text/html", int retryAfterInSeconds = 3600)
        {
            services.AddMaintenance(new MaintenanceWindow(enabler, response)
            {
                ContentType = contentType,
                RetryAfterInSeconds = retryAfterInSeconds
            });

            return services;
        }

        public static IApplicationBuilder UseMaintenance(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<MaintenanceMiddleware>();
        }
    }
}