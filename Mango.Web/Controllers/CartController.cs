using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

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
            return View(LoadCartBasedOnLoggenInUser());
        }

        private async Task<CartDto> LoadCartBasedOnLoggenInUser()
        {
            var userId = User.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub)?.Value;
            var response = await _cartService.GetCartByUserAsync(userId);
            if (response != null && response.IsSuccess)
            {
                return JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));
            }

            return default;
        }
    }
}
