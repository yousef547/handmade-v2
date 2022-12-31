using HandmadeStore.DataAccess.Repository.IRepository;
using HandmadeStore.Models;
using HandmadeStore.Models.Models;
using HandmadeStore.Models.Models.ViewModels;
using HandmadeStore.UI.Hubs;
using HandmadeStore.UI.Models;
using HandmadeStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using System.Security.Claims;

namespace HandmadeStore.UI.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<ReviewsHub> reviewHub;
        private readonly IHubContext<MassagesHub> massagesHub;


        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, IHubContext<ReviewsHub> reviewHub, IHubContext<MassagesHub> massagesHub)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            this.reviewHub = reviewHub;
            this.massagesHub = massagesHub;
        }

        public IActionResult Index()
        {
            HttpContext.Session.SetInt32(SD.CartSession, _unitOfWork.CartItem.GetPiecsCount());
            IEnumerable<Product> products = _unitOfWork.Product.GetAll(includeProperties: "Category,Brand");
            return View(products);
        }

        public IActionResult Details(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartFromDb = _unitOfWork.CartItem.GetFirstOrDefault(x => x.ApplicationUserId == userId && x.ProductId == productId);

            CartItem cartItem = new()
            {
                Count = cartFromDb == null ? 1 : cartFromDb.Count,
                ProductId = productId,
                Product = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == productId, includeProperties: "Category,Brand")
            };
            TempData["product_id"] = productId;
            IEnumerable<Review> reviews = _unitOfWork.Review.GetAll(r => r.ProductId == productId, includeProperties: "ApplicationUser");
            TempData["Reviews"] = reviews;

            return View(cartItem);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(CartItem cartItem)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            cartItem.ApplicationUserId = userId;
            var cartFromDb = _unitOfWork.CartItem.GetFirstOrDefault(x => x.ApplicationUserId == userId && x.ProductId == cartItem.ProductId);
            if (cartFromDb is null)
            {
                _unitOfWork.CartItem.Add(cartItem);
            }
            else
            {
                cartFromDb.Count = cartItem.Count;
                _unitOfWork.CartItem.Update(cartFromDb);
            }
            _unitOfWork.Save();
            HttpContext.Session.SetInt32(SD.CartSession, _unitOfWork.CartItem.GetPiecsCount());
            return RedirectToAction(nameof(Index));
        }
        //userId = (await _userManager.FindByNameAsync(User.Identity.Name)).Id


        [HttpPost]
        [Authorize]
        public IActionResult AddReview(Review review)
        {
            if (!string.IsNullOrEmpty(review.ReviewText))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                review.ApplicationUserId = userId;
                review.ProductId = (int)TempData["product_id"];
                review.ReviewDate = DateTime.Now;
                _unitOfWork.Review.Add(review);
                _unitOfWork.Save();
                reviewHub.Clients.All.SendAsync("LoadReviews", review.ProductId);
                return RedirectToAction("Details", new { productId = review.ProductId });
            }
            return RedirectToAction("Details", new { productId = review.ProductId });
        }


        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Send(MassageVM MassageVM)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var sender = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == userId);
            var receiver = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Name == "Admin");
            var massageToSender = new { sender = sender.Name, body = MassageVM.MassageTaxt };
            massagesHub.Clients.Users(receiver.Id).SendAsync("receiverMassage", massageToSender);
            return RedirectToAction("Index");
        }


        public IActionResult SetCulture(string lang,string returnUrl)
        {

            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
       CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(lang)),
       new CookieOptions { Expires = DateTimeOffset.UtcNow.AddMonths(1) });

            return LocalRedirect(returnUrl);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}