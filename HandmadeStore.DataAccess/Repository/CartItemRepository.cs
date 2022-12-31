using HandmadeStore.Data;
using HandmadeStore.Models.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HandmadeStore.DataAccess.Repository.IRepository
{
    internal class CartItemRepository : Repository<CartItem>, ICartItemRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor httpContextAccessor;

        public CartItemRepository(ApplicationDbContext context,IHttpContextAccessor httpContextAccessor) : base(context)
        {
            _context = context;
            this.httpContextAccessor = httpContextAccessor;
        }

        public void Update(CartItem cartItem)
        {
            _context.CartItems.Update(cartItem);
        }
        public int GetPiecsCount()
        {
            int PiecesCount = 0;
            var userId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = _context.CartItems.Where
                (c => c.ApplicationUserId == userId).ToList();
            if (cartItems != null)
            {
                foreach (var item in cartItems)
                {
                    PiecesCount += item.Count;
                }
            }
            return PiecesCount;
        }

    }
}
