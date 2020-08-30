using Hangfire;
using Microsoft.Extensions.Logging;
using Store.DAL;
using Store.Model.CartService;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Store.Scheduler.Domain.Services
{
    public class CartTimeoutService : ICartTimeoutService
    {
        private readonly ILogger<CartTimeoutService> _logger;

        private readonly ICartTimeoutServiceRepository _repository;

        public CartTimeoutService(
            ILogger<CartTimeoutService> logger,
            AppSettings appSettings,
            ICartTimeoutServiceRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [Queue("storetimeoutcarts")]
        [AutomaticRetry(Attempts = 1)]
        [DisableConcurrentExecution(3600 * 4)]
        public async Task ClearOldCarts(IJobCancellationToken cancellationToken)
        {
            //ToDo take daysLimit from AppSettings
            _logger.LogInformation($"{nameof(ClearOldCarts)} started in {DateTime.Now}");

            var webHooks = await _repository.GetHooks(daysLimit: 30);

            await _repository.RemoveOldCarts(daysLimit: 30);

            await CallHooks(webHooks);

            _logger.LogInformation($"{nameof(ClearOldCarts)} finished in {DateTime.Now}");
        }

        private async Task CallHooks(IEnumerable<Webhook> deletedCartsInfo)
        {
            using (var client = new HttpClient())
            {
                //ToDo call logic
            }
        }
    }
}
