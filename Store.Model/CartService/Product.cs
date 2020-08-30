using System.ComponentModel.DataAnnotations.Schema;

namespace Store.Model.CartService
{
    [Table("products")]
    public class Product
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("cost")]
        public decimal Cost { get; set; }

        [Column("for_bonus_points")]
        public bool ForBonusPoints { get; set; }
    }
}
