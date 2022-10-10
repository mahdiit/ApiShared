using ApiShared.Core;
using ApiShared.Core.Data;
using ApiShared.Core.Data.BaseClass;
using ApiShared.Core.Data.BaseInterface;
using ApiShared.Core.Middlewares;
using ApiShared.Sample.DbData;
using ApiShared.Sample.Implement;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ApiShared.Sample
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            ApiShared.Core.StaticParameters.CoreContext.ConfigurationsAssembly.Add(Assembly.GetExecutingAssembly());

            var services = new ServiceCollection();
            ConfigureServices(services);

            var provider = services.BuildServiceProvider();
            var repo = provider.GetService<IRepository<CoreContext>>();

            if (repo == null)
            {
                Console.WriteLine("No repo");
                return;
            }

            var result = repo.Query<SampleTable>().ToList();
            foreach (var item in result)
            {
                Console.WriteLine(item.Name);
            }

            Console.WriteLine("End");
            Console.ReadKey();
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddScoped<IRemoteIPResolver, Implement.RemoteIPResolver>();

            SqlConnectionStringBuilder myBuilder = new SqlConnectionStringBuilder();
            myBuilder.InitialCatalog = "PR";
            myBuilder.DataSource = "192.168.2.65";
            myBuilder.IntegratedSecurity = true;

            services.AddDbContext<CoreContext>(op => op.UseSqlServer(myBuilder.ConnectionString));
            services.AddScoped<IRepository<CoreContext>, Repository<CoreContext>>();
        }
    }
}