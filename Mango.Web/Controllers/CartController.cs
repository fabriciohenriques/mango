using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartBasedOnLoggenInUser());
        }

        private async Task<CartDto> LoadCartBasedOnLoggenInUser()
        {
            var response = await _cartService.GetCartByUserAsync();
            if (response != null && response.IsSuccess && response.Result != null)
                return JsonConvert.DeserializeObject<CartDto>(response.Result.ToString());

            return default;
        }
    }
}
