using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> Login(LoginRequestDto obj)
        //{
        //}

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDto obj)
        {
            var responseDto = await _authService.RegisterAsync(obj);
            if (responseDto != null && responseDto.IsSuccess)
            {
                TempData["success"] = "Registration Successful";
                return RedirectToAction(nameof(Login));
            }

            return View(obj);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            return View();
        }
    }
}
