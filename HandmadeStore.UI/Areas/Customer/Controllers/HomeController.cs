using HandmadeStore.DataAccess.Repository.IRepository;
using HandmadeStore.Models;
using HandmadeStore.Models.Models;
using HandmadeStore.UI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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

        public IActionResult Details(int id)
        {
            CartItem cartItem = new()
            {
                Count = 1,
                ProductId = id,
                Product = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == id, includeProperties: "Category,Brand")
            };
            return View(cartItem);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}