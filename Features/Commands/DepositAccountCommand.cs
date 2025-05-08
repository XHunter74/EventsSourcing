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

public class DepositAccountCommandHandler : BaseFeatureHandler, ICommandHandler<DepositAccountCommand, AccountAggregate>
{

    public DepositAccountCommandHandler(ILogger<DepositAccountCommandHandler> logger,
        EventStoreDbContext dbContext) : base(logger, dbContext)
    { }

    public async Task<AccountAggregate> HandleAsync(DepositAccountCommand command, CancellationToken cancellationToken)
    {
        var eventsData = await GetAggregateEvents(command.Id, cancellationToken: cancellationToken);

        if (!eventsData.Any())
        {
            throw new NotFoundException($"Account with id {command.Id} not found.");
        }

        var maxVersion = eventsData.Any() ? eventsData.Max(e => e.Version) + 1 : 1;

        var newEvent = new Event
        {
            AggregateId = command.Id,
            AggregateType = AggregateType.Account,
            EventType = EventType.MoneyDeposited,
            DecimalData = command.Amount,
            Version = maxVersion
        };
        await DbContext.Events.AddAsync(newEvent, cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken);

        var domainEvents = eventsData
            .Select(EventsMapper.ToDomainEvent)
            .Append(EventsMapper.ToDomainEvent(newEvent))
            .ToList();

        var account = new AccountAggregate();
        account.LoadsFromHistory(domainEvents);
        return account;
    }
}
