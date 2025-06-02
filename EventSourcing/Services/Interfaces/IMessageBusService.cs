namespace EventSourcing.Services.Interfaces
{
    public interface IMessageBusService
    {
        Task SendMessageToQueue(string queueName, object message);
    }
}
