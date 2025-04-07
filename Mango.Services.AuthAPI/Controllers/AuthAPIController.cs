using Mango.ServiceBus;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text;

namespace Mango.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IMessageBus _messageBus;
        private ResponseDto _response;
        private readonly ServiceBusConfig _serviceBusConfig;

        public AuthAPIController(
            IAuthService authService,
            IMessageBus messageBus,
            IOptions<ServiceBusConfig> serviceBusConfigOptions)
        {
            _authService = authService;
            _messageBus = messageBus;
            _response = new();
            _serviceBusConfig = serviceBusConfigOptions.Value;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
        {
            var errorMessage = await _authService.Register(model);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                _response.IsSuccess = false;
                _response.Message = errorMessage;
                return BadRequest(_response);
            }

            await _messageBus.PublishMessage(model.Email, _serviceBusConfig.RegisteredUsersQueue, _serviceBusConfig.ConnectionString);

            return Ok(new ResponseDto
            {
                IsSuccess = true,
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            var loginResponse = await _authService.Login(model);
            if (string.IsNullOrEmpty(loginResponse.Token))
            {
                _response.IsSuccess = false;
                _response.Message = "Username or password is invalid";
                return BadRequest(_response);
            }

            _response.Result = loginResponse;
            return Ok(_response);
        }

        [HttpPost("assignrole")]
        public async Task<bool> AssignRole([FromBody] AssignRoleDto model) => await _authService.AssignRole(model.Email, model.RoleName);
    }
}
