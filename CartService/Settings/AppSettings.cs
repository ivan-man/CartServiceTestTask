using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CartService.Settings
{
    public class AppSettings
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

        public readonly IConfiguration Configuration;

        public readonly string CartServiceConnectionString;

        public readonly AuthenticationSettings AuthenticationSettings = new AuthenticationSettings();

        public AppSettings()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"appsettings.{DefaultEnvironmentName}.json", optional: true, reloadOnChange: false);

            Configuration = builder.Build();

            CartServiceConnectionString = Configuration.GetConnectionString("CartService");

            Configuration.GetSection("Authentication").Bind(AuthenticationSettings);
        }
    }
}
