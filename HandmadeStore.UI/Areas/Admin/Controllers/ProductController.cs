using HandmadeStore.Data;
using HandmadeStore.DataAccess.Repository.IRepository;
using HandmadeStore.Models;
using HandmadeStore.Models.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HandmadeStore.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _host;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment host)
        {
            _unitOfWork = unitOfWork;
            _host = host;
        }

        public IActionResult Index()
        {
            return View();
        }


        ////Upsert Product
        //GET
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                Product = new(),
                CategoryList = _unitOfWork.Category.GetAll().Select(
                c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                }
                ),
                BrandList = _unitOfWork.Brand.GetAll().Select(
                b => new SelectListItem
                {
                    Text = b.Name,
                    Value = b.Id.ToString()
                }
                )
            };

            if (id == null || id == 0)
            {
                //Create Product
                //ViewBag.CategoryList = CategoryList;
                //ViewData["BrandList"] = BrandList;
                return View(productVM);
            }
            else
            {
                //Update Prodect
                productVM.Product = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == id);
                return View(productVM);
            }
        }

        //POST
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                string RootPath = _host.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var productsFolderPath = Path.Combine(RootPath, @"images\products");
                    var extension = Path.GetExtension(file.FileName);

                    //Delete old image file if exists
                    if (productVM.Product.ImageUrl != null)
                    {
                        var oldImagePath = Path.Combine(RootPath, productVM.Product.ImageUrl
                            .TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStreams = new FileStream(Path.Combine(productsFolderPath,
                        fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    productVM.Product.ImageUrl = @"\images\products\" + fileName + extension;

                }
                if (productVM.Product.Id == 0)
                {
                    //Create new product
                    _unitOfWork.Product.Add(productVM.Product);
                    _unitOfWork.Save();
                    TempData["success"] = "Product created successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    //Update product
                    _unitOfWork.Product.Update(productVM.Product);
                    _unitOfWork.Save();
                    TempData["success"] = "Product updated successfully";
                    return RedirectToAction("Index");
                }

            }
            return View(productVM);
        }


        #region API Endpoints
        //Get All Products Endpoint
        [HttpGet]
        public IActionResult GetAll()
        {
            var products = _unitOfWork.Product.GetAll(includeProperties: "Category,Brand");
            return Json(new { data = products });
        }

        //Delete Product Endpoint
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var product = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == id);
            if (product is null)
            {
                return Json(new { success = false, message = "Error while deleting product" });
            }
            var oldImagePath = Path.Combine(_host.WebRootPath, product.ImageUrl
                            .TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _unitOfWork.Product.Remove(product);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Product deleted successfully" });
        }
        #endregion
    }
}
