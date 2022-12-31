using HandmadeStore.Data;
using HandmadeStore.DataAccess.Repository.IRepository;
using HandmadeStore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandmadeStore.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
            //_context.Products.Include(p => p.Category).Include(p => p.Brand);
        }


        public void Update(Product product)
        {
            var productFromDb = _context.Products.Find(product.Id);
            if (productFromDb != null)
            {
                productFromDb.Name = product.Name;
                productFromDb.ArabicName = product.ArabicName;
                productFromDb.Description = product.Description;
                productFromDb.ArabicDescription = product.ArabicDescription;
                productFromDb.Price = product.Price;
                productFromDb.Price10Plus = product.Price10Plus;
                productFromDb.Price30Plus = product.Price30Plus;
                productFromDb.CategoryId = product.CategoryId;
                productFromDb.BrandId = product.BrandId;
                productFromDb.CreatedDate = product.CreatedDate;
                if (product.ImageUrl != null)
                {
                    productFromDb.ImageUrl = product.ImageUrl;
                }
            }
        }
    }
}
