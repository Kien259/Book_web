using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using MailKit.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyBook.Controllers
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

        public async Task<IActionResult> Index()
        {
            IEnumerable<Product> productList = await Task.Run(() => _unitOfWork.Product.GetAllAsync(includeProperties: "Category,CoverType"));
            return await Task.Run(() => View(productList)); 
        }

        public async Task<IActionResult> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return RedirectToAction(nameof(Index));
            }
            var products = await _unitOfWork.Product.SearchAsync(searchTerm);
            return View("SearchResults", products);
        }

        public async Task<IActionResult> Details(int productId)
        {
            ShoppingCart cartObj = new()
            {
                Count = 1,
                ProductId = productId,
                Product = await Task.Run(() => _unitOfWork.Product.GetFirstOrDefaultAsync(u => u.Id == productId, includeProperties: "Category,CoverType")),
            };

            return await Task.Run(() => View(cartObj));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity?)User.Identity;
            var claim = await Task.Run(() => claimsIdentity.FindFirst(ClaimTypes.NameIdentifier));
            shoppingCart.ApplicationUserId = claim.Value;

            var cartFromDb = await Task.Run(() => _unitOfWork.ShoppingCart.GetFirstOrDefaultAsync(
                                  u => u.ApplicationUserId == claim.Value && 
                                  u.ProductId == shoppingCart.ProductId));

            if(cartFromDb == null)
            {
                await _unitOfWork.ShoppingCart.AddAsync(shoppingCart);
                await _unitOfWork.SaveAsync();
                //Session Count
                HttpContext.Session.SetInt32(SD.SessionCart,
                    _unitOfWork.ShoppingCart.GetAllAsync(u => u.ApplicationUserId == claim.Value).ToList().Count);
            }
            else
            {
                await _unitOfWork.ShoppingCart.IncrementCount(cartFromDb, shoppingCart.Count);
                await _unitOfWork.SaveAsync();
            }

            return await Task.Run(() => RedirectToAction(nameof(Index)));
        }


        public async Task<IActionResult> Privacy()
        {
            return await Task.Run(() => View());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error()
        {
            return await Task.Run(() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }));
        }
    }
}