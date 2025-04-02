using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Mango.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Mango.Web.Service.IService;
using Newtonsoft.Json;

namespace Mango.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IProductService _productService;

    public HomeController(
        ILogger<HomeController> logger,
        IProductService productService)
    {
        _logger = logger;
        _productService = productService;
    }

    public async Task<IActionResult> Index()
    {
        var list = new List<ProductDto>();
        var response = await _productService.GetAllProductsAsync();
        if (response != null && response.IsSuccess)
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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
