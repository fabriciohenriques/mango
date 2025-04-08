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

        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartBasedOnLoggenInUser());
        }

        public async Task<IActionResult> Checkout()
        {
            return View(await LoadCartBasedOnLoggenInUser());
        }

        public async Task<IActionResult> Remove(int cartDetailId)
        {
            var response = await _cartService.RemoveFromCartAsync(cartDetailId);
            if (response != null && response.IsSuccess)
                TempData["success"] = "Cart updated successfully";
            else
                TempData["error"] = response?.Message;

            return RedirectToAction(nameof(CartIndex));
        }

        public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
        {
            var response = await _cartService.ApplyCouponAsync(cartDto.CartHeader);
            if (response != null && response.IsSuccess)
                TempData["success"] = "Coupon applied successfully";
            else
                TempData["error"] = response?.Message;

            return RedirectToAction(nameof(CartIndex));
        }

        public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
        {
            cartDto.CartHeader.CouponCode = string.Empty;
            var response = await _cartService.ApplyCouponAsync(cartDto.CartHeader);
            if (response != null && response.IsSuccess)
                TempData["success"] = "Coupon applied successfully";
            else
                TempData["error"] = response?.Message;

            return RedirectToAction(nameof(CartIndex));
        }

        public async Task<IActionResult> EmailCart()
        {
            var response = await _cartService.EmailCartAsync();

            if (response != null && response.IsSuccess)
                TempData["success"] = "Email will be processed and sent shortly.";
            else
                TempData["error"] = response?.Message;

            return RedirectToAction(nameof(CartIndex));
        }

        private async Task<CartDto> LoadCartBasedOnLoggenInUser()
        {
            var response = await _cartService.GetCartByUserAsync();
            if (response != null && response.IsSuccess && response.Result != null)
                return JsonConvert.DeserializeObject<CartDto>(response.Result.ToString());

            return new CartDto
            {
                CartHeader = new CartHeaderDto(),
                CartDetails = new List<CartDetailDto>()
            };
        }
    }
}
