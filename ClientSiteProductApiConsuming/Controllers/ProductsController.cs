using ClientSiteProductApiConsuming.Models;
using ClientSiteProductApiConsuming.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;

namespace ClientSiteProductApiConsuming.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService,
            IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
            _logger = logger;

        }

        [HttpGet("Dashboard/{productId?}")]
        [Authorize(Roles =("Admin"))]
        public async Task<IActionResult> Dashboard(int? productId = null)
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
            {
                // If no token is present, redirect to login
                return RedirectToAction("Login", "Account");
            }
            // Fetch products using the token
            var products = await _productService.GetProductsAsync(token);
            Product selectedProduct = null;
            if (productId.HasValue)
            {
                // Fetch the specific product for editing
                selectedProduct = await _productService.GetProductByIdAsync(productId.Value);

            }

            var model = new ProductViewModel
            {
                Products = products,
                Product = selectedProduct
            };
            // return Json(products);
            return View(model);
        }


        [HttpGet]
        public IActionResult AddProduct() => View();


        [HttpPost("AddProduct")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(Product product, IFormFile imageFile)
        {
            _logger.LogInformation("AddProduct method called.");
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Token is null or empty. Redirecting to Login");
                return RedirectToAction("Login", "Account");
            }

            try
            {
                _logger.LogInformation("ModelSate is valid. Calling AddProductAsync.");
                var result = await _productService.AddProductAsync(product, imageFile, token);

                if (result)
                {
                    _logger.LogInformation("Product added successfully.");
                    TempData["SuccessMessage"] = "Product added successfully!";
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    _logger.LogError("Error adding product. Service returned false.");
                    ModelState.AddModelError("", "Error adding product.");
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Exception occurred while adding product.");
                ModelState.AddModelError("", "An error occurred while adding the product.");
            }
            if (ModelState.IsValid)
            {

            }

            else
            {
                _logger.LogWarning("ModelState is invalid.");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning($"ModelState error: {error.ErrorMessage}");
                }
            }



            // return Json(product);
            return RedirectToAction("Dashboard");
        }
   
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProducts(Product product, IFormFile imageFile)
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            //if (!ModelState.IsValid)
            //{
            //    _logger.LogWarning("ModelState is invalid.");
            //    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            //    {
            //        _logger.LogWarning($"ModelState error: {error.ErrorMessage}");
            //    }
            //    return View(product);
            //}

            try
            {
                var result = await _productService.UpdateProductAsync(product, imageFile, token);

                if (result)
                {
                    TempData["SuccessMessage"] = "Product updated successfully!";
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    _logger.LogError("Error updating product. Service returned false.");
                    ModelState.AddModelError("", "Error updating product.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while updating product.");
                ModelState.AddModelError("", "An error occurred while updating the product.");
            }

            return View(product);
        }


        [HttpPost("DeleteProduct")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            _logger.LogInformation($"DeleteProduct method called for product ID: {id}");
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Token is null or empty. Redirecting to Login.");
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.DeleteAsync($"https://localhost:7269/api/Products/{id}");

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Product deleted successfully.");
                    TempData["SuccessMessage"] = "Product deleted successfully!";
                }
                else
                {
                    _logger.LogError("Error deleting product. Service returned false.");
                    TempData["ErrorMessage"] = "Error deleting product.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while deleting product.");
                TempData["ErrorMessage"] = "An error occurred while deleting the product.";
            }

            return RedirectToAction("Dashboard");
        }



    }
}

    


