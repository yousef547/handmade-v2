using HandmadeStore.Data;
using HandmadeStore.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Http;
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
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public ICartItemRepository CartItem { get; private set; }
        public IOrderDetailRepository OrderDetail { get; private set; }
        public IOrderHeaderRepository OrderHeader { get; private set; }
       public IReviewRepository Review { get; private set; }


        public UnitOfWork(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            Category = new CategoryRepository(context);
            Brand = new BrandRepository(context);
            Product = new ProductRepository(context);
            Shop = new ShopRepository(context);
            ApplicationUser = new ApplicationUserRepository(context);
            CartItem = new CartItemRepository(context, httpContextAccessor);
            OrderDetail = new OrderDetailRepository(context);
            OrderHeader = new OrderHeaderRepository(context);
            Review = new ReviewRepository(context);

        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
