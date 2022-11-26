using HandmadeStore.Data;
using HandmadeStore.DataAccess.Repository.IRepository;
using HandmadeStore.Models;
using HandmadeStore.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandmadeStore.DataAccess.Repository
{
    public class ShopRepository : Repository<Shop>, IShopRepository
    {
        private readonly ApplicationDbContext _context;

        public ShopRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }


        public void Update(Shop shop)
        {
            _context.Shops.Update(shop);
        }
    }
}
