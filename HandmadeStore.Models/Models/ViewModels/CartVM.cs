using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandmadeStore.Models.Models.ViewModels
{
    public class CartVM
    {
        public IEnumerable<CartItem> CartItems { get; set; }
        public double CartTotle { get; set; }
        public int PriceCount { get; set; }
    }
}
