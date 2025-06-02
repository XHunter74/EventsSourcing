using EventSourcing.Services.Interfaces;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace EventSourcing.Services
{
    public class RabbitMqService : IMessageBusService
    {
        private readonly IConnection _connection;

        public RabbitMqService(IConnection connection)
        {
            _connection = connection;
        }

        public Task SendMessageToQueue(string queueName, object message)
        {
            using var channel = _connection.CreateModel();
            channel.QueueDeclare(queue: queueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var messageStr = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(messageStr);
            channel.BasicPublish(exchange: "",
                                 routingKey: queueName,
                                 basicProperties: null,
                                 body: body);
            return Task.CompletedTask;
        }
    }
}
