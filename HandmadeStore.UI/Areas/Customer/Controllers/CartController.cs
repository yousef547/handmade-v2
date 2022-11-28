using HandmadeStore.DataAccess.Repository.IRepository;
using HandmadeStore.Models;
using HandmadeStore.Models.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HandmadeStore.UI.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private CartVM cartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            cartVM = new CartVM()
            {
                CartItems = _unitOfWork.CartItem.GetAll(x => x.ApplicationUserId == userId, includeProperties: "Product")
            };
            foreach (var item in cartVM.CartItems)
            {
                item.Price = GetPrice(item.Count, item.Product.Price, item.Product.Price10Plus, item.Product.Price30Plus);
                cartVM.CartTotle += (item.Price * item.Count);
                cartVM.PriceCount += item.Count;

            }
            return View(cartVM);
        }


        public IActionResult Increment(int cartId)
        {
            var cart = _unitOfWork.CartItem.GetFirstOrDefault(x => x.Id == cartId);
            cart.Count = cart.Count + 1;
            _unitOfWork.CartItem.Update(cart);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Decrement(int cartId)
        {
            var cart = _unitOfWork.CartItem.GetFirstOrDefault(x => x.Id == cartId);
            if (cart.Count <= 1)
            {
                _unitOfWork.CartItem.Remove(cart);
            }
            else
            {
                cart.Count = cart.Count - 1;
                _unitOfWork.CartItem.Update(cart);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int cartId)
        {
            var cart = _unitOfWork.CartItem.GetFirstOrDefault(x => x.Id == cartId);
            if(cart == null)
            {
                return NotFound();
            }
            _unitOfWork.CartItem.Remove(cart);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));

        }
        private double GetPrice(int count, double? price, double? price10plus, double? price30plus)
        {
            if (count <= 10)
            {
                return (double)price;
            }
            else
            {
                if (count <= 30)
                {
                    return (double)price10plus;
                }
                else
                {
                    return (double)price30plus;
                }
            }
        }
    }
}
