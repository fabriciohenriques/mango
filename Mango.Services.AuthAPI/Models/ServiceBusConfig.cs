namespace Mango.Services.AuthAPI.Models
{
    public class ServiceBusConfig
    {
        public required string ConnectionString { get; set; }
        public required string RegisteredUsersQueue { get; set; }
    }
}
