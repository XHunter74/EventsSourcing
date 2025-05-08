using CQRSMediatr.Interfaces;
using EventSourcing.Aggregates;
using EventSourcing.Data;
using EventSourcing.Exceptions;
using EventSourcing.Mappers;

namespace EventSourcing.Features.Commands;

public class DepositAccountCommand : ICommand<AccountAggregate>
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
}

public class DepositAccountCommandHandler : BaseAccountHandler, ICommandHandler<DepositAccountCommand, AccountAggregate>
{

    public DepositAccountCommandHandler(ILogger<DepositAccountCommandHandler> logger,
        EventStoreDbContext dbContext) : base(logger, dbContext)
    { }

    public async Task<AccountAggregate> HandleAsync(DepositAccountCommand command, CancellationToken cancellationToken)
    {
        var account = await GetAccountAggregateAsync(command.Id, null, cancellationToken);

        var newEvent = new Event
        {
            AggregateId = command.Id,
            AggregateType = AggregateType.Account,
            EventType = EventType.MoneyDeposited,
            DecimalData = command.Amount,
            Version = account.Version + 1
        };
        await DbContext.Events.AddAsync(newEvent, cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken);

        var domainEvent = EventsMapper.ToDomainEvent(newEvent);
        account.Apply(domainEvent);
        return account;
    }
}
