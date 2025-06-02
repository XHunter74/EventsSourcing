namespace EventSourcing.Services.Interfaces
{
    public interface IMessageBusService
    {
        Task SendMessageToQueue(string queueName, object message);
        void SubscribeToQueue(string queueName, Action<string> onMessage, CancellationToken cancellationToken = default);
    }
}
