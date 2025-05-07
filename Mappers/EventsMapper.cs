using EventSourcing.Data;
using EventSourcing.Events.Account;
using EventSourcing.Events;

namespace EventSourcing.Mappers;

public static class EventsMapper
{
    public static IEvent ToDomainEvent(Event entity)
    {
        return entity.EventType switch
        {
            EventType.AccountCreated => new AccountCreated(
                entity.Id,
                entity.AggregateId,
                entity.StringData!,
                entity.Version,
                entity.Created
            ),
            EventType.MoneyDeposited => new MoneyDeposited(
                entity.Id,
                entity.AggregateId,
                entity.DecimalData ?? 0,
                entity.Version,
                entity.Created
            ),
            EventType.MoneyWithdrawn => new MoneyWithdrawn(
                entity.Id,
                entity.AggregateId,
                entity.DecimalData ?? 0,
                entity.Version,
                entity.Created
            ),
            _ => throw new NotSupportedException($"Unknown event type: {entity.EventType}")
        };
    }
}
