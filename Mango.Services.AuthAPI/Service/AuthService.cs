using AutoMapper;
using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthAPI.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthService(
            AppDbContext db,
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = await _userManager.FindByEmailAsync(loginRequestDto.UserName);
            var isValid = false;

            if (user != null)
                isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);

            if (user != null && isValid)
            {
                //Generate token
                var response = _mapper.Map<LoginResponseDto>(user);
                response.Token = string.Empty;
                return response;
            }
            else
                return new LoginResponseDto();
        }

        public async Task<string?> Register(RegistrationRequestDto registrationRequestDto)
        {
            var user = _mapper.Map<ApplicationUser>(registrationRequestDto);

            try
            {
                var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);
                if (result.Succeeded)
                {
                    var userToReturn = await _userManager.FindByEmailAsync(registrationRequestDto.Email);

                    var userDto = _mapper.Map<UserDto>(user);

                    return string.Empty;
                }
                else
                    return result.Errors.FirstOrDefault()?.Description;
            }
            catch
            {
                return "Error encountered";
            }
        }
    }
}
