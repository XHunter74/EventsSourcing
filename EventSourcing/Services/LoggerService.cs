using EventSourcing.Models;
using EventSourcing.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.Services;

public class LoggerService : IHostedService
{
    private readonly ILogger<LoggerService> _logger;
    private readonly IMessageBusService _messageBusService;
    private CancellationTokenSource? _cts;

    public LoggerService(ILogger<LoggerService> logger, IMessageBusService messageBusService)
    {
        _logger = logger;
        _messageBusService = messageBusService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _messageBusService.SubscribeToQueue(Constants.LogQueueName, message =>
        {
            ProcessLogMessage(message);
        }, _cts.Token);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cts?.Cancel();
        return Task.CompletedTask;
    }

    private void ProcessLogMessage(string message)
    {
        var logMessage = JsonSerializer.Deserialize<LogMessageDto>(message);
        if (logMessage != null)
        {
            _logger.LogInformation($"Log: {logMessage.Message} - AggregateId: {logMessage.AggregateId} - EventType: {logMessage.EventType}");
        }
    }
}
