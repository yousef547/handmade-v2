using HandmadeStore.DataAccess.Repository.IRepository;
using HandmadeStore.Models;
using HandmadeStore.Models.Models;
using HandmadeStore.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace HandmadeStore.UI.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> products = _unitOfWork.Product.GetAll(includeProperties: "Category,Brand");
            return View(products);
        }

        public IActionResult Details(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartFromDb = _unitOfWork.CartItem.GetFirstOrDefault(x => x.ApplicationUserId == userId && x.Id == productId);

            CartItem cartItem = new()
            {
                Count = cartFromDb == null?0: cartFromDb.Count,
                ProductId = productId,
                Product = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == productId, includeProperties: "Category,Brand")
            };
            return View(cartItem);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(CartItem cartItem)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            cartItem.ApplicationUserId = userId;
            var cartFromDb = _unitOfWork.CartItem.GetFirstOrDefault(x => x.ApplicationUserId == userId && x.Id == cartItem.ProductId);
            if(cartFromDb is null)
            {
                _unitOfWork.CartItem.Add(cartItem);
            }else
            {
                _unitOfWork.CartItem.Update(cartItem);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        //userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id

       [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}