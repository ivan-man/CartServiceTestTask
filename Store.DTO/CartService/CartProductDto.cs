using System;
using System.Collections.Generic;
using System.Text;

namespace Store.DTO.CartService
{
    public class CartProductDto
    {
        public DateTime Created { get; set; }
        public DateTime Edited { get; set; }
        public int Number { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
    }
}
