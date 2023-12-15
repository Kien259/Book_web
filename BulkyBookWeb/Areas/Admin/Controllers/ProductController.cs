using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = SD.Role_Admin)]
	public class ProductController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
		{
			_unitOfWork = unitOfWork;
			_webHostEnvironment = webHostEnvironment;
		}

		public async Task<IActionResult> Index()
		{
			var objProductList = _unitOfWork.Product.GetAllAsync();
			return await Task.Run(() => View(objProductList));
		}

		public async Task<IActionResult> Create()
		{
			return await Task.Run(() => View());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(CoverType obj)
		{
			if (!ModelState.IsValid) ModelState.AddModelError("name", "Cover Type name is not valid");

			if (ModelState.IsValid)
			{
				await _unitOfWork.CoverType.AddAsync(obj);
				await _unitOfWork.SaveAsync();
				TempData["success"] = "Cover Type created successfully!";

				return await Task.Run(() => RedirectToAction("Index"));
			}

			return await Task.Run(() => View(obj));
		}

		public async Task<IActionResult> Upsert(int? id)
		{
			ProductVM productVM = new()
			{
				Product = new(),
				CategoryList = await Task.Run(() => _unitOfWork.Category.GetAllAsync().Select(i => new SelectListItem
				{
					Text = i.Name,
					Value = i.Id.ToString()
				})),
				CoverTypeList = await Task.Run(() => _unitOfWork.CoverType.GetAllAsync().Select(i => new SelectListItem
				{
					Text = i.Name,
					Value = i.Id.ToString()
				}))
			};

			if (id == null || id == 0)
			{
				return await Task.Run(() => View(productVM));
			}
			else
			{
				productVM.Product = await Task.Run(() => _unitOfWork.Product.GetFirstOrDefaultAsync(u => u.Id == id));
				return await Task.Run(() => View(productVM));
			}

		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Upsert(ProductVM obj, IFormFile? file)
		{

			if (ModelState.IsValid)
			{
				var tempPath = Path.GetTempPath();

				if (file != null)
				{
					var fileName = Guid.NewGuid().ToString();
					var uploads = tempPath;
					var extention = Path.GetExtension(file.FileName);
					var saveFile = Path.Combine(uploads, fileName + extention);

					if (obj.Product.ImageUrl != null)
					{
						var oldImagePath = Path.Combine(tempPath, obj.Product.ImageUrl.Trim('\\'));
						if (System.IO.File.Exists(oldImagePath))
						{
							await Task.Run(() => System.IO.File.Delete(oldImagePath));
						}
					}

					using (var laborImg = Image.Load(file.OpenReadStream()))
					{
						laborImg.Mutate(x => x.Resize(300, 300));
						laborImg.Save(saveFile);
						laborImg.Dispose();
					}

					byte[] imageArray = System.IO.File.ReadAllBytes(saveFile);
					string base64ImageRepresentation = Convert.ToBase64String(imageArray);

					obj.Product.ImageUrl = $"data:image/{extention};base64,{base64ImageRepresentation}";
				}

				if (obj.Product.Id == 0)
				{
					await _unitOfWork.Product.AddAsync(obj.Product);
				}
				else
				{
					await Task.Run(() => _unitOfWork.Product.Update(obj.Product));
				}

				await _unitOfWork.SaveAsync();
				TempData["success"] = "Product created successfully!";

				return await Task.Run(() => RedirectToAction("Index"));
			}

			return await Task.Run(() => View(obj));
		}

		#region API CALL
		public IActionResult GetAll()
		{
			var productList = _unitOfWork.Product.GetAllAsync(includeProperties:"Category,CoverType");
			return Json( new { data = productList } );
		}
		[HttpDelete]
		public  async Task<IActionResult> Delete(int? id)
		{
			var obj = await _unitOfWork.Product.GetFirstOrDefaultAsync(u => u.Id == id);
			if (obj == null)
			{
				return await Task.Run(() => Json(new { success = false, message = "Error while deleting" }));
			}

			var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
			if (System.IO.File.Exists(oldImagePath))
			{
				await Task.Run(() => System.IO.File.Delete(oldImagePath));
			}

			await _unitOfWork.Product.RemoveAsync(obj);
			await _unitOfWork.SaveAsync();
			return await Task.Run(() => Json(new { success = true, message = "Delete Successful" }));
		}
		#endregion
	}
}
