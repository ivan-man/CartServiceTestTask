using Store.DTO.CartService;
using System.Collections.Generic;
using System.Threading.Tasks;
using Store.Model.CartService;

namespace Store.DAL
{
    public interface ICartServiceRepository : IBaseRepository
    {
        /// <summary>
        /// Получение корзины.
        /// </summary>
        /// <param name="buyerId">Id покупателя.</param>
        /// <param name="page">Номер страницы для отображения.</param>
        /// <param name="pageSize">Размер страницы при отображении.</param>
        /// <param name="orderBy">Поля для сортировки.</param>
        /// <returns><inheritdoc cref="CartDto"/></returns>
        public Task<CartDto> GetCart(string buyerId, int? page = null, int? pageSize = null, string orderBy = "");

        /// <summary>
        /// Добавление товаров в корзину.
        /// </summary>
        /// <param name="buyerId">Id покупателя.</param>
        /// <param name="products"><see cref="Product.Id"/>></param>
        public Task AddProducts(string buyerId, IEnumerable<AddProductToCartDto> products);

        /// <summary>
        /// Удаление товаров из корзины.
        /// </summary>
        /// <param name="buyerId">Id покупателя.</param>
        /// <param name="products"><see cref="Product.Id"/>></param>
        public Task RemoveProducts(string buyerId, IEnumerable<AddProductToCartDto> products);

        /// <summary>
        /// Добавление вебхуков.
        /// </summary>
        /// <param name="urls">Ссылки для добваления.</param>
        /// <param name="buyerId">Id покупателя.</param>
        public Task AddWebhooks(IEnumerable<string> urls, string buyerId = "");

        /// <summary>
        /// Удаление вебхуков.
        /// </summary>
        /// <param name="urls">Ссылки для добваления.</param>
        /// <param name="buyerId">Id покупателя.</param>
        public Task RemoveWebhooks(IEnumerable<string> urls, string buyerId = "");
    }
}
