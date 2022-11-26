using HandmadeStore.Data;
using HandmadeStore.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandmadeStore.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public ICategoryRepository Category { get; private set; }
        public IBrandRepository Brand { get; private set; }
        public IProductRepository Product { get; private set; }
        public IShopRepository Shop { get; private set; }


        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Category = new CategoryRepository(context);
            Brand = new BrandRepository(context);
            Product = new ProductRepository(context);
            Shop = new ShopRepository(context);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
