using Common;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Store.DTO.CartService
{
    /// <summary>
    /// Dto с информацией о корзине и товарами в ней.
    /// </summary>
    public class CartDto
    {
        public Guid? BuyerId { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Edited { get; set; }

        public PaginationWrapper<CartProductDto> Products { get; set; }
    }
}
