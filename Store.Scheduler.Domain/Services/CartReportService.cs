using Hangfire;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Store.Scheduler.Domain.Services
{
    public class CartReportService : ICartReportService
    {
        private readonly ILogger<CartReportService> _logger;
        private readonly ICartReportService _repository;

        public CartReportService(
           ILogger<CartReportService> logger,
           AppSettings appSettings,
           ICartReportService repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [Queue("cartreports")]
        [AutomaticRetry(Attempts = 1)]
        [DisableConcurrentExecution(3600 * 4)]
        public async Task GenerateReport(IJobCancellationToken cancellationToken)
        { 

        }
    }
}
