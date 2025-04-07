namespace Mango.Services.EmailAPI.Models
{
    public class ServiceBusConfig
    {
        public required string ConnectionString { get; set; }
        public required string EmailShoppingCartQueue { get; set; }
        public required string RegisteredUsersQueue { get; set; }
    }
}
