using HandmadeStore.Data;
using HandmadeStore.DataAccess.Repository.IRepository;
using HandmadeStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace HandmadeStore.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BrandController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public BrandController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Brand> brands = _unitOfWork.Brand.GetAll();
            return View(brands);
        }

        ////Create Brand
        //GET
        public IActionResult Create()
        {
            return View();
        }

        //POST
        [HttpPost]
        public IActionResult Create(Brand brand)
        {
            if (!string.IsNullOrEmpty(brand.Name))
            {
                var duplicatedBrand = _unitOfWork.Brand.GetFirstOrDefault(p => p.Name.ToLower() == brand.Name.ToLower());
                if (duplicatedBrand != null)
                {
                    //ModelState.AddModelError(String.Empty, "This brand name is duplicated.");
                    ModelState.AddModelError("name", "This brand name is duplicated.");
                }
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Brand.Add(brand);
                _unitOfWork.Save();
                TempData.Add("success", "Brand created successfully");
                return RedirectToAction("Index");
            }
            return View(brand);
        }

        ////Update Brand
        //GET
        public IActionResult Update(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            var brand = _unitOfWork.Brand.GetFirstOrDefault(p => p.Id == id);
            if (brand == null)
            {
                return NotFound();
            }
            return View(brand);
        }

        //POST
        [HttpPost]
        public IActionResult Update(Brand brand)
        {
            var brandNameFromDb = _unitOfWork.Brand.GetFirstOrDefault(p => p.Id == brand.Id).Name;
            if (!string.IsNullOrEmpty(brand.Name))
            {
                var duplicatedBrand = _unitOfWork.Brand.GetFirstOrDefault(p => p.Name.ToLower() == brand.Name.ToLower());
                if (duplicatedBrand != null && duplicatedBrand.Name.ToLower() != brandNameFromDb.ToLower())
                {
                    //ModelState.AddModelError(String.Empty, "This brand name is duplicated.");
                    ModelState.AddModelError("name", "This brand name is duplicated.");
                }
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Brand.ClearChangeTracking();
                _unitOfWork.Brand.Update(brand);
                _unitOfWork.Save();
                TempData.Add("success", "Brand updated successfully");
                return RedirectToAction("Index");
            }
            return View(brand);
        }

        ////Delete Brand
        //GET
        public IActionResult Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            var brand = _unitOfWork.Brand.GetFirstOrDefault(p => p.Id == id);
            if (brand == null)
            {
                return NotFound();
            }
            return View(brand);
        }

        //POST
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int id)
        {
            var brand = _unitOfWork.Brand.GetFirstOrDefault(p => p.Id == id);
            if (brand is null)
            {
                return NotFound();
            }
            _unitOfWork.Brand.Remove(brand);
            _unitOfWork.Save();
            TempData.Add("success", "Brand deleted successfully");
            return RedirectToAction("Index");
        }
    }
}
