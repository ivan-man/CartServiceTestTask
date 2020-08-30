using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store.Model.CartService
{
    /// <summary>
    /// Сущность для хранения информации об урлах вебхуков.
    /// Можно нагрузить разными свойствами, типа типов запросов, срок годности и прочее.
    /// <see cref="Webhook.BuyerId"/> - необязательное свойство, если он не указан - то значит хук общий для всех.
    /// </summary>
    [Table("webhooks")]
    public class Webhook
    {
        [Column("buyer_id")]
        public Guid BuyerId { get; set; }

        [Column("url")]
        public string Url { get; set; }
    }
}
