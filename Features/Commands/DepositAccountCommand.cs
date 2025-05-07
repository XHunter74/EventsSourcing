using CQRSMediatr.Interfaces;
using EventSourcing.Aggregates;
using EventSourcing.Data;
using EventSourcing.Mappers;
using Microsoft.EntityFrameworkCore;

namespace EventSourcing.Features.Commands;

public class DepositAccountCommand : ICommand<AccountAggregate>
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
}

public class DepositAccountCommandHandler : ICommandHandler<DepositAccountCommand, AccountAggregate>
{
    private readonly EventStoreDbContext _dbContext;

    public DepositAccountCommandHandler(EventStoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AccountAggregate> HandleAsync(DepositAccountCommand command)
    {
        var maxVersion = await _dbContext.Events
            .Where(e => e.AggregateId == command.Id)
            .AsNoTracking()
            .MaxAsync(e => e.Version);
        maxVersion++;
        var newEvent = new Event
        {
            AggregateId = command.Id,
            AggregateType = AggregateType.Account,
            EventType = EventType.MoneyDeposited,
            DecimalData = command.Amount,
            Version = maxVersion
        };
        await _dbContext.Events.AddAsync(newEvent);
        await _dbContext.SaveChangesAsync();
        var events = (await _dbContext.Events
            .Where(e => e.AggregateId == command.Id)
            .OrderBy(e => e.Created)
            .AsNoTracking()
            .ToListAsync())
            .Select(EventsMapper.ToDomainEvent)
            .ToList();
        var account = new AccountAggregate();
        account.LoadsFromHistory(events);
        return account;
    }
}
