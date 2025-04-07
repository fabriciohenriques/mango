namespace Mango.ServiceBus
{
    public interface IMessageBus
    {
        Task PublishMessage(object message, string topicQueueName, string connStr);
    }
}
