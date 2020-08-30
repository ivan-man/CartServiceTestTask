using Common;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Store.DAL;
using Store.Scheduler.Domain.Services;
using System;

namespace Store.Scheduler.Domain.Configure
{
    public static class ConfigureStoreShedulerExtension
    {
        public static void ConfigureSheduler(this IServiceCollection services, AppSettings appSettings)
        {
            services.AddHostedService<HostService>();

            services.AddSingleton(appSettings);
            
            var dateTimeService = new DateTimeService();
            services.AddSingleton(dateTimeService);

            services.AddSingleton<ICartTimeoutServiceRepository>(new CartTimeoutServiceRepository(dateTimeService, appSettings.CartServiceConnectionString));

            services.AddTransient<ICartTimeoutService, CartTimeoutService>();
            services.AddTransient<ICartReportService, CartReportService>();

            services.AddLogging(loggingBuilder =>
            {
#if DEBUG
                loggingBuilder.ClearProviders();
                loggingBuilder.AddConsole();
#else
                //ToDo Add any logger
#endif
            });

            services.AddHangfire(c => c.UsePostgreSqlStorage(appSettings.HangfireConnectionString, new PostgreSqlStorageOptions()
            {
                DistributedLockTimeout = TimeSpan.FromHours(2)
            }));

            services.AddHangfireServer((option) =>
            {
                option.ServerName = "StoreTimeoutCarts";
                option.ServerTimeout = TimeSpan.FromHours(1);
                option.Queues = new[] { "storetimeoutcarts" };
                option.WorkerCount = 1;
            });

            services.AddHangfireServer((option) =>
            {
                option.ServerName = "CartReports";
                option.ServerTimeout = TimeSpan.FromHours(1);
                option.Queues = new[] { "cartreports" };
                option.WorkerCount = 1;
            });

            services.AddSingleton<JobActivator, ContainerJobActivator>();
        }
    }

    public class ContainerJobActivator : JobActivator
    {
        private readonly IServiceProvider _serviceProvider;

        public ContainerJobActivator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override object ActivateJob(Type objectType)
        {
            return ActivatorUtilities.CreateInstance(_serviceProvider, objectType);
        }
    }
}
