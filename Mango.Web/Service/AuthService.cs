using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public class AuthService : BaseService, IAuthService
    {
        public AuthService(IHttpClientFactory httpClientFactory)
            : base(httpClientFactory)
        {
        }

        public async Task<ResponseDto?> AssignRoleAsync(AssignRoleDto assignRoleDto) =>
            await SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.POST,
                Data = assignRoleDto,
                Url = SD.AuthAPIBase + "api/assignrole",
            });

        public async Task<ResponseDto?> LoginAsync(LoginRequestDto loginRequestDto) =>
            await SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.POST,
                Data = loginRequestDto,
                Url = SD.AuthAPIBase + "api/login",
            });

        public async Task<ResponseDto?> RegisterAsync(RegistrationRequestDto registrationRequestDto) =>
            await SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.POST,
                Data = registrationRequestDto,
                Url = SD.AuthAPIBase + "api/register",
            });
    }
}
