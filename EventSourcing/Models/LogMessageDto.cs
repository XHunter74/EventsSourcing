using EventSourcing.Data;

namespace EventSourcing.Models;

public class LogMessageDto
{
    public Guid AggregateId { get; set; }
    public AggregateType AggregateType { get; set; }
    public EventType EventType { get; set; }
    public string Message { get; set; } = string.Empty;
}
