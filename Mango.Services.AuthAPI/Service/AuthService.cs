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
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthService(
            AppDbContext db,
            IJwtTokenGenerator jwtTokenGenerator,
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _jwtTokenGenerator = jwtTokenGenerator;
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
                var response = _mapper.Map<LoginResponseDto>(user);
                response.Token = _jwtTokenGenerator.GenerateToken(user);
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

                    await AssignRole(userToReturn, "user");

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

        public async Task<bool> AssignRole(string email, string? roleName)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return await AssignRole(user, roleName);
        }

        private async Task<bool> AssignRole(ApplicationUser? user, string? roleName)
        {
            if (user != null && !string.IsNullOrWhiteSpace(roleName))
            {
                var upperRoleName = roleName.ToUpper().Trim();
                if (!await _roleManager.RoleExistsAsync(upperRoleName))
                {
                    var createRoleResult = await _roleManager.CreateAsync(new IdentityRole(upperRoleName));
                    if (!createRoleResult.Succeeded)
                        return false;
                }

                var result = await _userManager.AddToRoleAsync(user, upperRoleName);

                return result.Succeeded;
            }

            return false;
        }
    }
}
