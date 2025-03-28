namespace Mango.Services.AuthAPI.Models.Dto
{
    public class LoginResponseDto : UserDto
    {
        public string Token { get; set; }
    }
}
