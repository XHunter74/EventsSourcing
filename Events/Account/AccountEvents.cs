namespace EventSourcing.Events.Account;

public record AccountCreated(Guid Id, Guid AccountId, string OwnerName, int Version, DateTime OccurredOn) : IEvent;
public record MoneyDeposited(Guid Id, Guid AccountId, decimal Amount, int Version, DateTime OccurredOn) : IEvent;
public record MoneyWithdrawn(Guid Id, Guid AccountId, decimal Amount, int Version, DateTime OccurredOn) : IEvent;
