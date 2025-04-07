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
        private readonly ServiceBusProcessor _registeredUsersProcessor;

        public AzureServiceBusConsumer(
            EmailService emailService,
            IOptions<ServiceBusConfig> serviceBusConfigOptions)
        {
            _emailService = emailService;
            _serviceBusConfig = serviceBusConfigOptions.Value;

            var client = new ServiceBusClient(_serviceBusConfig.ConnectionString);
            _emailCartProcessor = client.CreateProcessor(_serviceBusConfig.EmailShoppingCartQueue);
            _registeredUsersProcessor = client.CreateProcessor(_serviceBusConfig.RegisteredUsersQueue);
        }

        public async Task Start()
        {
            _emailCartProcessor.ProcessMessageAsync += OnEmailCartRequestReceived;
            _emailCartProcessor.ProcessErrorAsync += ErrorHandler;
            await _emailCartProcessor.StartProcessingAsync();

            _registeredUsersProcessor.ProcessMessageAsync += OnRegisteredUserRequestReceived;
            _registeredUsersProcessor.ProcessErrorAsync += ErrorHandler;
            await _registeredUsersProcessor.StartProcessingAsync();
        }

        private async Task OnRegisteredUserRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            var email = JsonConvert.DeserializeObject<string>(body);

            try
            {
                await _emailService.LogRegisteredUser(email);
                await args.CompleteMessageAsync(message);
            }
            catch
            {
                throw;
            }
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

            await _registeredUsersProcessor.StopProcessingAsync();
            await _registeredUsersProcessor.DisposeAsync();
        }
    }
}
