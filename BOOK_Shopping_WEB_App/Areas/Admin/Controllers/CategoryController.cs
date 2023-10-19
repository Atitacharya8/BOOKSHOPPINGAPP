using BOOKSHOPPING.DataAccess.Data;
using BOOKSHOPPING.DataAccess.Repository.IRepository;
using BOOKSHOPPING.Models;
using BOOKSHOPPING.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BOOK_Shopping_WEB_App.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Category> objCategoryList = _unitOfWork.Category.GetAll().ToList();

            return View(objCategoryList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The DisplayOrder cannot exactly match the name.");
            }

            //if (obj.Name.ToLower() == "test")
            // {
            //     ModelState.AddModelError("", "Test is an invalid value.");
            // }

            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            return View();

        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            //Method 1 --> only works with primary key
            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);

            //Method 2-- > use link operation that can work with any values such as "Name" as Name.contains() or Name = ""
            //Category? categoryFromDb1 = _db.Categories.FirstOrDefault(u => u.Id == id);

            //Method 3-- > use link operation
            //Category? categoryfromdb2 = _db.Categories.Where(u => u.Id == id).FirstOrDefault();



            if (categoryFromDb == null)
            {
                return NotFound();
            }

            return View(categoryFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category updated successfully";
                return RedirectToAction("Index");
            }
            return View();

        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            //Method 1 --> only works with primary key
            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);

            //Method 2-- > use link operation that can work with any values such as "Name" as Name.contains() or Name = ""
            //Category? categoryFromDb1 = _db.Categories.FirstOrDefault(u => u.Id == id);

            //Method 3-- > use link operation
            //Category? categoryfromdb2 = _db.Categories.Where(u => u.Id == id).FirstOrDefault();



            if (categoryFromDb == null)
            {
                return NotFound();
            }

            return View(categoryFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id) // or Delete(Category? id)
        {
            Category? obj = _unitOfWork.Category.Get(u => u.Id == id);

            if (obj == null)
            {
                return NotFound();
            }

            _unitOfWork.Category.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
