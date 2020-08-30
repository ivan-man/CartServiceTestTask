using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Store.DAL;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CartService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebhookController : ControllerBase
    {
        private ICartServiceRepository _repository;
        private ILogger _logger;

        public WebhookController(ILogger<WebhookController> logger, ICartServiceRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        /// <summary>
        /// Добавление урлов для вебхуков.
        /// </summary>
        /// <param name="buyerId">Владелец корзины.</param>
        /// <param name="urls">Список урлов.</param>
        [HttpPut]
        [Authorize]
        [Route("{buyerId}")]
        public async Task<IActionResult> Subscribe([FromRoute] string buyerId, [FromBody] IEnumerable<string> urls)
        {
            await _repository.Subscribe(urls, buyerId);

            return Ok();
        }

        /// <summary>
        /// Добавление урлов для вебхуков.
        /// </summary>
        /// <param name="urls">Список урлов.</param>
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Subscribe([FromBody] IEnumerable<string> urls)
        {
            await _repository.Subscribe(urls);

            return Ok();
        }

        /// <summary>
        /// Удаление урлов для вебхуков.
        /// </summary>
        /// <param name="buyerId">Владелец корзины.</param>
        /// <param name="urls">Список урлов.</param>
        [HttpDelete]
        [Authorize]
        [Route("{buyerId}")]
        public async Task<IActionResult> Unsubscribe([FromRoute] string buyerId, [FromBody] IEnumerable<string> urls)
        {
            await _repository.Unsubscribe(urls, buyerId);

            return Ok();
        }

        /// <summary>
        /// Удаление урлов для вебхуков.
        /// </summary>
        /// <param name="urls">Список урлов.</param>
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Unsubscribe([FromBody] IEnumerable<string> urls)
        {
            await _repository.Unsubscribe(urls);

            return Ok();
        }
    }
}
