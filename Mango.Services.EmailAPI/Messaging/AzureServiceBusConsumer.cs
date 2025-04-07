using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPI.Models;
using Microsoft.Extensions.Options;

namespace Mango.Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer
    {
        private readonly ServiceBusConfig _serviceBusConfig;
        private readonly ServiceBusProcessor _emailCartProcessor;

        public AzureServiceBusConsumer(
            IOptions<ServiceBusConfig> serviceBusConfigOptions)
        {
            _serviceBusConfig = serviceBusConfigOptions.Value;

            var client = new ServiceBusClient(_serviceBusConfig.ConnectionString);
            _emailCartProcessor = client.CreateProcessor(_serviceBusConfig.EmailShoppingCartQueue);
        }
    }
}
