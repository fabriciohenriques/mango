﻿namespace Mango.Services.ShoppingCartAPI.Models
{
    public class ServiceBusConfig
    {
        public required string ConnectionString { get; set; }
        public required string EmailShoppingCartQueue { get; set; }
    }
}
