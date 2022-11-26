﻿using HandmadeStore.Data;
using HandmadeStore.DataAccess.Repository.IRepository;
using HandmadeStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace HandmadeStore.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> categories = _unitOfWork.Category.GetAll();
            return View(categories);
        }

        ////Create Category
        //GET
        public IActionResult Create()
        {
            return View();
        }

        //POST
        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (!string.IsNullOrEmpty(category.Name))
            {
                var duplicatedCategory = _unitOfWork.Category.GetFirstOrDefault(p => p.Name.ToLower() == category.Name.ToLower());
                if (duplicatedCategory != null)
                {
                    //ModelState.AddModelError(String.Empty, "This category name is duplicated.");
                    ModelState.AddModelError("name", "This category name is duplicated.");
                }
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(category);
                _unitOfWork.Save();
                TempData.Add("success", "Category created successfully");
                return RedirectToAction("Index");
            }
            return View(category);
        }

        ////Update Category
        //GET
        public IActionResult Update(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            var category = _unitOfWork.Category.GetFirstOrDefault(p => p.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        //POST
        [HttpPost]
        public IActionResult Update(Category category)
        {
            var categoryNameFromDb = _unitOfWork.Category.GetFirstOrDefault(p => p.Id == category.Id).Name;
            if (!string.IsNullOrEmpty(category.Name))
            {
                var duplicatedCategory = _unitOfWork.Category.GetFirstOrDefault(p => p.Name.ToLower() == category.Name.ToLower());
                if (duplicatedCategory != null && duplicatedCategory.Name.ToLower() != categoryNameFromDb.ToLower())
                {
                    //ModelState.AddModelError(String.Empty, "This category name is duplicated.");
                    ModelState.AddModelError("name", "This category name is duplicated.");
                }
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.ClearChangeTracking();
                _unitOfWork.Category.Update(category);
                _unitOfWork.Save();
                TempData.Add("success", "Category updated successfully");
                return RedirectToAction("Index");
            }
            return View(category);
        }

        ////Delete Category
        //GET
        public IActionResult Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            var category = _unitOfWork.Category.GetFirstOrDefault(p => p.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        //POST
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int id)
        {
            var category = _unitOfWork.Category.GetFirstOrDefault(p => p.Id == id);
            if (category is null)
            {
                return NotFound();
            }
            _unitOfWork.Category.Remove(category);
            _unitOfWork.Save();
            TempData.Add("success", "Category deleted successfully");
            return RedirectToAction("Index");
        }
    }
}
