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
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderHeaderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(OrderHeader orderHeader)
        {
            _context.OrderHeaders.Update(orderHeader);
        }

        public void UpdateOrderPayment(int id, string sessionId, string paymentIntentId)
        {
            var OrderPayment = _context.OrderHeaders.FirstOrDefault(x => x.Id == id);
            OrderPayment.PaymentDate = DateTime.Now;
            OrderPayment.SessionId = sessionId;
            OrderPayment.PaymentIntentId = paymentIntentId;
        }

        public void UpdateStatus(int id, string OrderStatus, string paymentStatus = null)
        {
            var orderHeader = _context.OrderHeaders.FirstOrDefault(x => x.Id == id);
            if(orderHeader != null)
            {
                orderHeader.OrderStatus = OrderStatus;
                if(paymentStatus != null)
                {
                    orderHeader.PaymentStatus = paymentStatus;
                }
            }
        }
    }
}
