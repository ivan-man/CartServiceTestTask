using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Store.Scheduler.Domain.Services
{
    public class HostService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;

        public HostService(ILogger<HostService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"[{nameof(HostService)}] has been started.....");

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"[{nameof(HostService)}] has been stopped.....");

            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }
    }
}
