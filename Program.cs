using COUB.Utils.Extensions;
using COUB.Utils.Messaging;
using COUB.WeblessUtils;
using ITSK.COP.Utils.DI;
using ITSK.COP.Utils.Logging;
using ITSK.COP.Utils.ORM;
using ITSK.COUB;
using ITSK.AzimuthPT.JsonFlowReader.Service.logTest;
using ITSK.AzimuthPT.JsonFlowReader.Service.ObjectsToSettings;
using log4net.Repository.Hierarchy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using CoubModuleSettings = ITSK.AzimuthPT.JsonFlowReader.Service.ObjectsToSettings.CoubModuleSettings;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

namespace ITSK.AzimuthPT.JsonFlowReader.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                IConfiguration config = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .AddEnvironmentVariables()
                            .Build();

                        CreateHostBuilder(args, config).Build().Run();
            }
            catch (Exception e)
            {
            }

        }

        public static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration) =>
            Host.CreateDefaultBuilder(args)
            .UseWindowsService(options =>
            {
                options.ServiceName = "WinService_ITSK_AzimuthPT_JsonFlowReader_Service";
            })
            .ConfigureServices(services =>
            {
                services.Configure<CoubModuleSettings>(configuration.GetSection("CoubModuleSettings"));
                services.Configure<DataServiceSettings>(configuration.GetSection("DataServiceSettings"));
                services.AddDataServiceAsSingleton(configuration)
                .AddSingleton<IHttpContextAccessor, FakeHttpContextAccessor>()
                .AddCoub(configuration)
                .AddLogging();
                services.AddHostedService<Worker>();
                DIProvider.SetServiceProvider(services.BuildServiceProvider());
            });
    }
}
