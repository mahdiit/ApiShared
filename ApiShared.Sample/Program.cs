using ApiShared.Core;
using ApiShared.Core.Data;
using ApiShared.Core.Data.BaseClass;
using ApiShared.Core.Data.BaseInterface;
using ApiShared.Core.Data.Excel;
using ApiShared.Core.Data.Excel.ExcelHelper;
using ApiShared.Core.Middlewares;
using ApiShared.Sample.DbData;
using ApiShared.Sample.Implement;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging;

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

            var result1 = repo.Query<CHARACTERISTIC_DETAIL>().ToList();
            var result2 = repo.Query<CHARACTERISTIC>().ToList();

            var excelBuilder = new ListExcelBuilder() { IsRTL = false };
            var excelResult = excelBuilder
                .Create()
                .AddSheet("CH_1", result1)
                .SetColumnProvider(new DefaultColumnProvider() { HasAttributeOnly = true })
                .AddSheet("CH_2", result2)
                .Build();

            File.WriteAllBytes(AppDomain.CurrentDomain.BaseDirectory + "\\resutl1.xlsx", excelResult);


            var sqlExcelExport = provider.GetService<ISqlExcelExport>();
            if (sqlExcelExport == null)
            {
                Console.WriteLine("No service");
                return;
            }

            SqlConnectionStringBuilder myBuilder = new SqlConnectionStringBuilder();
            myBuilder.InitialCatalog = "TitanErp";
            myBuilder.DataSource = "192.168.2.65";
            myBuilder.IntegratedSecurity = true;

            var result3 = sqlExcelExport.GetFile<CHARACTERISTIC_DETAIL>(
                new SqlQueryDto(myBuilder.ConnectionString, "sELECT * FROM MasterData.CHARACTERISTIC_DETAIL"),
                new SqlExcelExportOption(new List<string> { "CH_3" })
            {
                Rtl = false,
                AutoFitColumns = true,
                //SheetTitles = new List<string>() { "sHEET 1" },
                HasRowNumber = false,                
            });

            if(result3 == null || result3.ErrorCode != 0)
            {
                Console.WriteLine("error");
                return;
            }

            if (result3.Data != null)
                File.WriteAllBytes(AppDomain.CurrentDomain.BaseDirectory + "\\resutl2.xlsx", result3.Data);
            else
                Console.WriteLine("no data");

            var exportedFile = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "\\resutl2.xlsx");
            using var info = exportedFile.OpenRead();
            ExcelListImporter excelListImporter = new ExcelListImporter();

            excelListImporter.SetStream(info).LoadFile();
            excelListImporter.SetCurrentSheet("CH_3");

            var result4 = excelListImporter.GetList<CHARACTERISTIC_DETAIL>();
            Console.WriteLine(result4.Count);
               
            Console.WriteLine("End");
            Console.ReadKey();
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddScoped<IRemoteIPResolver, Implement.RemoteIPResolver>();
            services.AddLogging(opt =>
            {                
                opt.AddConsole();
            });

            SqlConnectionStringBuilder myBuilder = new SqlConnectionStringBuilder();
            myBuilder.InitialCatalog = "TitanErp";
            myBuilder.DataSource = "192.168.2.65";
            myBuilder.IntegratedSecurity = true;

            services.AddDbContext<CoreContext>(op => op.UseSqlServer(myBuilder.ConnectionString));
            services.AddScoped<IRepository<CoreContext>, Repository<CoreContext>>();
            services.AddTransient<ISqlExcelExport, SqlExcelExport>();
        }
    }
}