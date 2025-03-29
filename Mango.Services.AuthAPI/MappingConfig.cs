using AutoMapper;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;

namespace Mango.Services.AuthAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<RegistrationRequestDto, ApplicationUser>()
                .ForMember(d => d.UserName, o => o.MapFrom(s => s.Email))
                .ForMember(d => d.NormalizedUserName, o => o.MapFrom(s => s.Email.ToUpper()))
                .ForMember(d => d.Email, o => o.MapFrom(s => s.Email))
                .ForMember(d => d.NormalizedEmail, o => o.MapFrom(s => s.Email.ToUpper()))
                .ForMember(d => d.PhoneNumber, o => o.MapFrom(s => s.PhoneNumber))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name));

                config.CreateMap<ApplicationUser, UserDto>();
            });

            return mappingConfig;
        }
    }
}
