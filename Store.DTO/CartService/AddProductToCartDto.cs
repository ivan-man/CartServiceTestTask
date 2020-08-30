using System;
using System.Collections.Generic;
using System.Text;

namespace Store.DTO.CartService
{
    /// <summary>
    /// DTO для добавления и удаления продуктов в корзину.
    /// </summary>
    public class AddProductToCartDto
    {
        public int ProductId { get; set; }
        public int Number { get; set; }
    }
}
