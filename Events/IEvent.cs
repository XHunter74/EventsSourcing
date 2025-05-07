namespace EventSourcing.Events;

public interface IEvent
{
    Guid Id { get; }
    int Version { get; }
    DateTime OccurredOn { get; }
}
