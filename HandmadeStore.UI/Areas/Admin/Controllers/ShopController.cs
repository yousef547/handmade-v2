using HandmadeStore.Data;
using HandmadeStore.DataAccess.Repository.IRepository;
using HandmadeStore.Models;
using HandmadeStore.Models.Models;
using HandmadeStore.Models.Models.ViewModels;
using HandmadeStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HandmadeStore.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Moderator)]
    public class ShopController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShopController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }


        ////Upsert Product
        //GET
        public IActionResult Upsert(int? id)
        {
            Shop shop = new();

            if (id == null || id == 0)
            {
                return View(shop);
            }
            else
            {
                //Update Prodect
                shop = _unitOfWork.Shop.GetFirstOrDefault(p => p.Id == id);
                return View(shop);
            }
        }

        //POST
        [HttpPost]
        public IActionResult Upsert(Shop shop)
        {
            if (ModelState.IsValid)
            {
                if (shop.Id == 0)
                {
                    //Create new product
                    _unitOfWork.Shop.Add(shop);
                    _unitOfWork.Save();
                    TempData["success"] = "shop created successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    //Update product
                    _unitOfWork.Shop.Update(shop);
                    _unitOfWork.Save();
                    TempData["success"] = "shop updated successfully";
                    return RedirectToAction("Index");
                }

            }
            return View(shop);
        }


        #region API Endpoints
        //Get All Products Endpoint
        [HttpGet]
        public IActionResult GetAll()
        {
            var shops = _unitOfWork.Shop.GetAll();
            return Json(new { data = shops });
        }

        //Delete Product Endpoint
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var shop = _unitOfWork.Shop.GetFirstOrDefault(p => p.Id == id);
            if (shop is null)
            {
                return Json(new { success = false, message = "Error while deleting product" });
            }
         
            _unitOfWork.Shop.Remove(shop);
            _unitOfWork.Save();
            return Json(new { success = true, message = "shop deleted successfully" });
        }
        #endregion
    }
}
