using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;
        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        public async Task<IActionResult> CouponIndex()
        {
            var list = new List<CouponDto>();
            var response = await _couponService.GetAllCouponsAsync();
            if (response != null && response.IsSuccess)
                list = JsonConvert.DeserializeObject<List<CouponDto>>(response.Result.ToString());
            else
                TempData["error"] = response?.Message;

            return View(list);
        }

        public async Task<IActionResult> CouponCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CouponCreate(CouponDto model)
        {
            if (ModelState.IsValid)
            {
                var response = await _couponService.CreateCouponAsync(model);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Coupon created.";
                    return RedirectToAction(nameof(CouponIndex));
                }
                else
                    TempData["error"] = response?.Message;
            }

            return View(model);
        }

        public async Task<IActionResult> CouponDelete(int couponId)
        {
            var response = await _couponService.GetCouponByIdAsync(couponId);
            if (response != null && response.IsSuccess)
            {
                var model = JsonConvert.DeserializeObject<CouponDto>(response.Result.ToString());
                return View(model);
            }
            else
                TempData["error"] = response?.Message;
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CouponDelete(CouponDto model)
        {
            var response = await _couponService.DeleteCouponAsync(model.CouponId);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Coupon deleted.";
                return RedirectToAction(nameof(CouponIndex));
            }
            else
                TempData["error"] = response?.Message;

            return View(model);
        }
    }
}
