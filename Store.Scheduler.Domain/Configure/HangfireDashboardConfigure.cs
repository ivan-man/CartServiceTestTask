using Hangfire;
using Store.Scheduler.Domain.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Store.Scheduler.Domain.Configure
{
    public class HangfireDashboardConfigure
    {
        public static void Configure()
        {
            var timezone = TimeZoneInfo.Utc;

            try
            {
                timezone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Moscow");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            RecurringJob.AddOrUpdate<CartTimeoutService>(x => x.ClearOldCarts(JobCancellationToken.Null), Cron.Daily(0, 1), timezone);
            //RecurringJob.AddOrUpdate<CartTimeoutService>(x => x.ClearOldCarts(JobCancellationToken.Null), Cron.Minutely(), timezone);

            RecurringJob.AddOrUpdate<CartReportService>(x => x.GenerateReport(JobCancellationToken.Null), Cron.Daily(6, 1), timezone);
            //RecurringJob.AddOrUpdate<CartReportService>(x => x.GenerateReport(JobCancellationToken.Null), Cron.Minutely(), timezone);
        }
    }
}
