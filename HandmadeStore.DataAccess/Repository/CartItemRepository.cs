﻿using HandmadeStore.Data;
using HandmadeStore.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandmadeStore.DataAccess.Repository.IRepository
{
    internal class CartItemRepository : Repository<CartItem>, ICartItemRepository
    {
        private readonly ApplicationDbContext _context;
        public CartItemRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(CartItem cartItem)
        {
            _context.CartItems.Update(cartItem);
        }
    }
}