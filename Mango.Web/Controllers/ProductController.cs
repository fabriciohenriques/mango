using Mango.Web.Models;
using Mango.Web.Service;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;

namespace Mango.Web.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> ProductIndex()
        {
            var list = new List<ProductDto>();
            var response = await _productService.GetAllProductsAsync();
            if (response != null && response.IsSuccess)
                list = JsonConvert.DeserializeObject<List<ProductDto>>(response.Result.ToString());
            else
                TempData["error"] = response?.Message;

            return View(list);
        }

        public IActionResult ProductCreate() => View();

        [HttpPost]
        public async Task<IActionResult> ProductCreate(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                var response = await _productService.CreateProductAsync(model);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Product created.";
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                    TempData["error"] = response?.Message;
            }

            return View(model);
        }

        public async Task<IActionResult> ProductUpdate(int productId) => await GetSingleProduct(productId);

        [HttpPost]
        public async Task<IActionResult> ProductUpdate(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                var response = await _productService.UpdateProductAsync(model);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Product updated.";
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                    TempData["error"] = response?.Message;
            }

            return View(model);
        }

        public async Task<IActionResult> ProductDelete(int productId) => await GetSingleProduct(productId);

        [HttpPost]
        public async Task<IActionResult> ProductDelete(ProductDto model)
        {
            var response = await _productService.DeleteProductAsync(model.ProductId);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Product deleted.";
                return RedirectToAction(nameof(ProductIndex));
            }
            else
                TempData["error"] = response?.Message;

            return View(model);
        }

        private async Task<IActionResult> GetSingleProduct(int productId)
        {
            var response = await _productService.GetProductAsync(productId);
            if (response != null && response.IsSuccess)
            {
                var model = JsonConvert.DeserializeObject<ProductDto>(response.Result.ToString());
                return View(model);
            }
            else
                TempData["error"] = response?.Message;

            return NotFound();
        }
    }
}
