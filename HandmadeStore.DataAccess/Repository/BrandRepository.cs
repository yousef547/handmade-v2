using HandmadeStore.Data;
using HandmadeStore.DataAccess.Repository.IRepository;
using HandmadeStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandmadeStore.DataAccess.Repository
{
    public class BrandRepository : Repository<Brand>, IBrandRepository
    {
        private readonly ApplicationDbContext _context;

        public BrandRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }


        public void Update(Brand brand)
        {
            _context.Brands.Update(brand);
        }
    }
}
