using Hangfire;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Store.Scheduler.Domain.Services
{
    public interface ICartTimeoutService
    {
        public Task ClearOldCarts(IJobCancellationToken cancellationToken);
    }
}
