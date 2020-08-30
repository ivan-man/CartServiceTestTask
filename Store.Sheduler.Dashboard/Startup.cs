using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Store.Scheduler.Domain.Configure;

namespace Store.Sheduler.Dashboard
{
    public class Startup
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

        private string HangfireConnectionString { get; set; }

        public Startup(IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"appsettings.{DefaultEnvironmentName}.json", optional: true, reloadOnChange: false);

            Configuration = builder.Build();

            HangfireConnectionString = Configuration.GetConnectionString("Hangfire");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHangfire(c => c.UsePostgreSqlStorage(HangfireConnectionString));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new MyAuthorizationFilter() }
            });

            HangfireDashboardConfigure.Configure();
        }
        
        public class MyAuthorizationFilter : IDashboardAuthorizationFilter
        {
            public bool Authorize(DashboardContext context)
            {
                // Allow all authenticated users to see the Dashboard (potentially dangerous).
                return true;
            }
        }
    }
}
