using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPI.Models;
using Mango.Services.EmailAPI.Models.Dto;
using Mango.Services.EmailAPI.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly EmailService _emailService;
        private readonly ServiceBusConfig _serviceBusConfig;
        private readonly ServiceBusProcessor _emailCartProcessor;

        public AzureServiceBusConsumer(
            EmailService emailService,
            IOptions<ServiceBusConfig> serviceBusConfigOptions)
        {
            _emailService = emailService;
            _serviceBusConfig = serviceBusConfigOptions.Value;

            var client = new ServiceBusClient(_serviceBusConfig.ConnectionString);
            _emailCartProcessor = client.CreateProcessor(_serviceBusConfig.EmailShoppingCartQueue);
        }

        public async Task Start()
        {
            _emailCartProcessor.ProcessMessageAsync += OnEmailCartRequestReceived;
            _emailCartProcessor.ProcessErrorAsync += ErrorHandler;
            await _emailCartProcessor.StartProcessingAsync();
        }

        private async Task OnEmailCartRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            var objMessage = JsonConvert.DeserializeObject<CartDto>(body);

            try
            {
                await _emailService.EmailCartAndLog(objMessage);
                await args.CompleteMessageAsync(message);
            }
            catch
            {
                throw;
            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        public async Task Stop()
        {
            await _emailCartProcessor.StopProcessingAsync();
            await _emailCartProcessor.DisposeAsync();
        }
    }
}
