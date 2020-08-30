using Store.Model.CartService;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Store.DAL
{
    public interface ICartTimeoutServiceRepository : IBaseRepository
    {
        public Task RemoveOldCarts(int daysLimit);
        public Task<List<Webhook>> GetHooks(int daysLimit);
    }
}
