using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Mango.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Mango.Web.Service.IService;
using Newtonsoft.Json;

namespace Mango.Web.Controllers;

public class HomeController : Controller
{
    private readonly ICartService _cartService;
    private readonly IProductService _productService;

    public HomeController(
        ICartService cartService,
        IProductService productService)
    {
        _cartService = cartService;
        _productService = productService;
    }

    public async Task<IActionResult> Index()
    {
        var list = new List<ProductDto>();
        var response = await _productService.GetAllProductsAsync();
        if (response != null && response.IsSuccess && response.Result != null)
            list = JsonConvert.DeserializeObject<List<ProductDto>>(response.Result.ToString());
        else
            TempData["error"] = response?.Message;

        return View(list);
    }

    [Authorize]
    public async Task<IActionResult> ProductDetails(int productId)
    {
        var response = await _productService.GetProductAsync(productId);
        if (response != null && response.IsSuccess)
        {
            var productDto = JsonConvert.DeserializeObject<ProductDto>(response.Result.ToString());
            return View(productDto);
        }
        else
            TempData["error"] = response?.Message;

        return RedirectToAction(nameof(Index));
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> ProductDetails(ProductDto productDto)
    {
        var cartDto = new CartDto
        {
            CartHeader = new CartHeaderDto(),
            CartDetails =
            [
                new()
                {
                    Count = productDto.Count,
                    ProductId = productDto.ProductId,
                }
            ]
        };
        var response = await _cartService.UpsertCartAsync(cartDto);
        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Product added to cart successfully.";
            return RedirectToAction(nameof(Index));
        }
        else
        {
            TempData["error"] = response?.Message;
            return View(productDto);
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
