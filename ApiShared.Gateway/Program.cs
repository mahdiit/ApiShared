using Microsoft.IdentityModel.Logging;
using Ocelot.Administration;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace ApiShared.Gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddJsonFile("ocelot.json");

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddOcelot().AddAdministration("/cfgedit", "6y4ALTNTey8xueQJPTMfkYNx");

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseWebSockets();            
            app.UseRouting();
            app.UseCors(o => o.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            app.UseAuthorization();
            app.UseOcelot();

            app.MapControllers();
            app.Run();
        }
    }
}