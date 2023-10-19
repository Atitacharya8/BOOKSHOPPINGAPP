using BOOKSHOPPING.DataAccess.Data;
using BOOKSHOPPING.DataAccess.Repository.IRepository;
using BOOKSHOPPING.Models;
using BOOKSHOPPING.Models.ViewModels;
using BOOKSHOPPING.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BOOK_Shopping_WEB_App.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment; //this helps to access wwwwebroot folder for static files
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
           
            return View(objProductList);
        }

        public IActionResult Upsert(int? id) //for both insert and update for product because we are making create and update in same page
        {
            //1.  ViewBag.categoryList = CategoryList;
            //2.  ViewData["categoryList"] = CategoryList;
            //3. 
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category
               .GetAll().Select(u => new SelectListItem //EF core projections
               {
                   Text = u.Name,
                   Value = u.Id.ToString()
               }),
                Product = new Product()
            };

            if (id == null || id == 0)
            {
                //go to create
                return View(productVM);

            }
            else
            {
                //got to update
                productVM.Product=_unitOfWork.Product.Get(u=>u.Id== id);
                return View(productVM);
            }

        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            //Custom Validation
            //if (obj.Name == obj.DisplayOrder.ToString())
            //{
            //    ModelState.AddModelError("name", "The DisplayOrder cannot exactly match the name.");
            //}

            //if (obj.Name.ToLower() == "test")
            // {
            //     ModelState.AddModelError("", "Test is an invalid value.");
            // }

            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if(file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\Product");

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        //delete the old image
                        var oldImagePath= 
                            Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));

                        if(System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    productVM.Product.ImageUrl = @"\images\Product\" + fileName;
                }
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                _unitOfWork.Save();
                TempData["success"] = "Product created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category
               .GetAll().Select(u => new SelectListItem //EF core projections
               {
                   Text = u.Name,
                   Value = u.Id.ToString()
               });
                return View(productVM);
            }

        }

        //public IActionResult Edit(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    //Method 1 --> only works with primary key
        //    Product? productFromDb = _unitOfWork.Product.Get(u => u.Id == id);

        //    //Method 2-- > use link operation that can work with any values such as "Name" as Name.contains() or Name = ""
        //    //Category? categoryFromDb1 = _db.Categories.FirstOrDefault(u => u.Id == id);

        //    //Method 3-- > use link operation
        //    //Category? categoryfromdb2 = _db.Categories.Where(u => u.Id == id).FirstOrDefault();



        //    if (productFromDb == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(productFromDb);
        //}

        //[HttpPost]
        //public IActionResult Edit(Product obj)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.Product.Update(obj);
        //        _unitOfWork.Save();
        //        TempData["success"] = "Product updated successfully";
        //        return RedirectToAction("Index");
        //    }
        //    return View();

        //}

     
       
        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {

            var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error in Deleting Product" });
            }
            var oldImagePath =
                             Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }


            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Product Deleted Successfully" });
        }


        #endregion
    }
}
