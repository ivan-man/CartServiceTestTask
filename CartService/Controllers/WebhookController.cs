using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public WebhookController(ICartServiceRepository repository)
        {
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
        public async Task<IActionResult> AddWebhooks([FromRoute] string buyerId, [FromBody] IEnumerable<string> urls)
        {
            await _repository.AddWebhooks(urls, buyerId);

            return Ok();
        }

        /// <summary>
        /// Добавление урлов для вебхуков.
        /// </summary>
        /// <param name="urls">Список урлов.</param>
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> AddWebhooks([FromBody] IEnumerable<string> urls)
        {
            await _repository.AddWebhooks(urls);

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
        public async Task<IActionResult> RemoveWebhooks([FromRoute] string buyerId, [FromBody] IEnumerable<string> urls)
        {
            await _repository.RemoveWebhooks(urls, buyerId);

            return Ok();
        }

        /// <summary>
        /// Удаление урлов для вебхуков.
        /// </summary>
        /// <param name="urls">Список урлов.</param>
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> RemoveWebhooks([FromBody] IEnumerable<string> urls)
        {
            await _repository.RemoveWebhooks(urls);

            return Ok();
        }
    }
}
