using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store.Model.CartService
{
    /// <summary>
    /// Запись о товаре в корзине.
    /// </summary>
    [Table("carts")]
    public class Cart
    {
        [Column("buyer_id")]
        public Guid BuyerId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        /// <summary>
        /// Количество.
        /// </summary>
        [Column("number")]
        public int Number { get; set; }

        /// <summary>
        /// Время добавления.
        /// </summary>
        [Column("created")]
        public DateTime Created { get; set; }

        /// <summary>
        /// Время добавления.
        /// </summary>
        [Column("edited")]
        public DateTime Edited { get; set; }
    }
}
