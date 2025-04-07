using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;

namespace Mango.ServiceBus
{
    public class MessageBus : IMessageBus
    {
        public async Task PublishMessage(object message, string topicQueueName, string connStr)
        {
            await using var client = new ServiceBusClient(connStr);
            var sender = client.CreateSender(topicQueueName);
            try
            {
                var messageBody = JsonConvert.SerializeObject(message);
                var serviceBusMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(messageBody))
                {
                    CorrelationId = Guid.NewGuid().ToString(),
                };
                await sender.SendMessageAsync(serviceBusMessage);
            }
            finally
            {
                await sender.DisposeAsync();
                await client.DisposeAsync();
            }
        }
    }
}
