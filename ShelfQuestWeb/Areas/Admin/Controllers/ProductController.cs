using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using ShelfQuest.DataAccess.Repository.IRepository;
using ShelfQuest.Models;
using ShelfQuest.Models.ViewModels;

namespace ShelfQuestWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }


        public IActionResult Index()
        {
            List<Product> obj = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();

            return View(obj);
        }

        public IActionResult Upsert(int? id)
        {

            ProductVM ProductView = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                //Create : Update
                Product = (id == 0 || id == null) ? new() : _unitOfWork.Product.GetFirstorDefault(u => u.Id == id),
            };
            return View(ProductView);
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRoothPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    //Generating a random string
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    //saving file path to the string.
                    string productPath = Path.Combine(wwwRoothPath, @"images\product");

                    //Deleting Old image if exists.
                    if (!string.IsNullOrEmpty(productVM.Product.ImageURL))
                    {
                       
                        var oldImagePath = Path.Combine(wwwRoothPath, productVM.Product.ImageURL.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    //Copying the file to the destination path.
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }  

                    productVM.Product.ImageURL = Path.Combine(@"\images\product\", fileName);
                }

                //If creating a new product
                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);

                }
                //If editing an existing product.
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);

                }
                _unitOfWork.Save();
                TempData["success"] = "Product Created/Updated Successfully";
                return RedirectToAction("Index", "Product");

            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productVM);
            }
        }


        #region API CALLS  

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> obj = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
            return Json(new { data = obj });
        }

        [HttpDelete] 
        public IActionResult Delete(int? id)
        {
            var wwwRootPath = _webHostEnvironment.WebRootPath;
            var productToBeDeleted = _unitOfWork.Product.GetFirstorDefault(u => u.Id == id);

            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while Deleting." });
            }

            var oldImagePath = Path.Combine(wwwRootPath, productToBeDeleted.ImageURL.TrimStart('\\'));

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Product Deleted Successfully." });
        }
        #endregion
    }
}
