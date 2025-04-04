﻿using AutoMapper;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;

namespace Mango.Services.ShoppingCartAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CartHeaderDto, CartHeader>();
                config.CreateMap<CartHeader, CartHeaderDto>();
                config.CreateMap<CartDetail, CartDetailDto>();
                config.CreateMap<CartDetailDto, CartDetail>();
            });

            return mappingConfig;
        }
    }
}
