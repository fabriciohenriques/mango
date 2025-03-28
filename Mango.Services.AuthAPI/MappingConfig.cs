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
                .ForMember(d => d.UserName, o => o.MapFrom(src => src.Email))
                .ForMember(d => d.NormalizedEmail, o => o.MapFrom(src => src.Email.ToUpper()));

                config.CreateMap<ApplicationUser, UserDto>();
            });

            mappingConfig.AssertConfigurationIsValid();
            return mappingConfig;
        }
    }
}
