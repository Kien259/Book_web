using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork  = unitOfWork;
        }

        public async Task<IActionResult>Index()
        {
            var objCategoryList = _unitOfWork.Category.GetAllAsync();

            return await Task.Run(() => View(objCategoryList));
        }


        //GET
        public async Task<IActionResult> Create()
        {

            return await Task.Run(() => View());
        }


        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The displayOrder cannot exactly match the Name.");
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.Category.AddAsync(obj);
                _unitOfWork.SaveAsync();
                TempData["success"] = "Category created successfully";

                return RedirectToAction("Index");

            }

            return View(obj);
        }

        // GET 
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id == 0) return NotFound();

            var categoryFromDbFirst = await _unitOfWork.Category.GetFirstOrDefaultAsync(u => u.Id == id);

            if (categoryFromDbFirst == null) return NotFound();
            
            return await Task.Run(() =>View(categoryFromDbFirst));
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The displayOrder cannot exactly match the Name."); 
            }

            if (ModelState.IsValid)
            {
                await Task.Run(() => _unitOfWork.Category.Update(obj));
                await _unitOfWork.SaveAsync();
                TempData["success"] = "Category updated successfully";
                return await Task.Run(() => RedirectToAction("Index"));

            }

            return await Task.Run(() => View(obj));
        }





        // GET
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0) return NotFound();

            var categoryFromDBFirst = await Task.Run(() => _unitOfWork.Category.GetFirstOrDefaultAsync(u => u.Id == id));

            if (categoryFromDBFirst == null) return NotFound();

            return await Task.Run(() => View(categoryFromDBFirst));
        }

        //POST
        [HttpPost, ActionName("Delete")] 
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int? id)
        {
            var obj = await Task.Run(() => _unitOfWork.Category.GetFirstOrDefaultAsync(u => u.Id == id));
            

            if (obj == null) return NotFound();

            await _unitOfWork.Category.RemoveAsync(obj);
            await _unitOfWork.SaveAsync();
            TempData["success"] = "Category deleted successfully";

            return await Task.Run(() => RedirectToAction("Index"));
        }
    }
}