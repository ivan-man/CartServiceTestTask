using Hangfire;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Store.DAL;
using Store.DTO.CartReportService;
using System.Threading.Tasks;

namespace Store.Scheduler.Domain.Services
{
    public class CartReportService : ICartReportService
    {
        private readonly ILogger<CartReportService> _logger;
        private readonly ICartReportServiceRepository _repository;

        public CartReportService(
           ILogger<CartReportService> logger,
           AppSettings appSettings,
           ICartReportServiceRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [Queue("cartreports")]
        [AutomaticRetry(Attempts = 1)]
        [DisableConcurrentExecution(3600 * 4)]
        public async Task GenerateReport(IJobCancellationToken cancellationToken)
        {
            var reportData = await _repository.GetReportData();

            // Эти методы лучше вынести в отдельный сервис и передавать сюда DI, 
            // чтобы всегда иметь возможность подмены реализации.
            // Решил оставить этот момент вне рамок этой задачи.
            var reportString = PrepareReport(reportData);
            await SendReport(reportString);
        }

        /// <summary>
        /// Подготовка отчета.
        /// </summary>
        private string PrepareReport(CartReportDto reportData)
        {
            return JsonConvert.SerializeObject(reportData);
        }

        /// <summary>
        /// Сохранить/отправить/вывести.
        /// </summary>
        private async Task SendReport(string reportString)
        {
            _logger.LogInformation(reportString);
        }
    }
}
