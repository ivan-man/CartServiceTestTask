using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Store.Scheduler.Domain;
using Store.Scheduler.Domain.Configure;
using System;
using System.Threading.Tasks;

namespace Store.Sheduler.Server
{
    class Program
    {

#if DEBUG
        private const string DefaultEnvironmentName = "debug";
#elif RELEASE
        private const string DefaultEnvironmentName = "release";
#elif DOCKER
        private const string DefaultEnvironmentName = "docker";
#else
        private const string DefaultEnvironmentName = "";
#endif

        static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .UseSystemd()
                .ConfigureServices((hostContext, services) =>
                {
                    services.ConfigureSheduler(GetAppSettings());
                });

            await builder.RunConsoleAsync();
        }

        static AppSettings GetAppSettings()
        {
            var appSettings = new AppSettings();

            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"appsettings.{DefaultEnvironmentName}.json", optional: true, reloadOnChange: false);

            appSettings.Configuration = builder.Build();

            appSettings.CartServiceConnectionString = appSettings.Configuration.GetConnectionString("CartService");
            appSettings.HangfireConnectionString = appSettings.Configuration.GetConnectionString("Hangfire");

            return appSettings;
        }
    }
}
