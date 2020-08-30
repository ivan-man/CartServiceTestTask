using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.DAL;
using Store.DTO.CartService;

namespace CartService.Controllers
{
    [ApiController]
    [Route("{buyerId}/[controller]")]
    public class CartController : ControllerBase
    {
        private ICartServiceRepository _repository;

        public CartController(ICartServiceRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Получение данных о корзине пользователя.
        /// </summary>
        /// <param name="buyerId">Id покупателя.</param>
        /// <param name="page">Номер страницы для отображения.</param>
        /// <param name="pageSize">Размер страницы при отображении.</param>
        /// <param name="orderBy">Поля по которым надо отсортировать продукты.</param>
        /// <returns><inheritdoc cref="CartDto"/></returns>
        [HttpGet]
        [Authorize]
        [Route("")]
        public async Task<ActionResult<CartDto>> GetCart(
            string buyerId,
            int? page = 1,
            int? pageSize = 20,
            string orderBy = "name"
            )
        {
            var cart = await _repository.GetCart(buyerId, page, pageSize, orderBy);

            return cart;
        }

        /// <summary>
        /// Добавление товаров в корзину.
        /// </summary>
        /// <param name="buyerId">Id покупателя.</param>
        /// <param name="products">DTO для добавления и удаления продуктов в корзину.</param>
        [HttpPost]
        [Authorize]
        [Route("addProducts")]
        public async Task<IActionResult> AddProducts([FromRoute] string buyerId, [FromBody] IEnumerable<AddProductToCartDto> products)
        {
            await _repository.AddProducts(buyerId, products);

            return Ok();
        }

        /// <summary>
        /// Удаление товаров из корзины.
        /// </summary>
        /// <param name="buyerId">Id покупателя.</param>
        /// <param name="products">DTO для добавления и удаления продуктов в корзину.</param>
        [HttpPost]
        [Authorize]
        [Route("removeProducts")]
        public async Task<IActionResult> RemoveProducts(string buyerId, IEnumerable<AddProductToCartDto> products)
        {
            await _repository.RemoveProducts(buyerId, products);

            return Ok();
        }
    }
}
