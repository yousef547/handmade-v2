using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandmadeStore.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        IBrandRepository Brand { get; }
        IProductRepository Product { get; }
        IShopRepository Shop { get; }
        void Save();
    }
}
