using CQRSMediatr.Interfaces;
using EventSourcing.Data;
using EventSourcing.Mappers;
using EventSourcing.Models;

namespace EventSourcing.Features.Commands;

public class DepositAccountCommand : ICommand<AccountDto>
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
}

public class DepositAccountCommandHandler : BaseAccountHandler, ICommandHandler<DepositAccountCommand, AccountDto>
{

    public DepositAccountCommandHandler(ILogger<DepositAccountCommandHandler> logger,
        EventStoreDbContext dbContext) : base(logger, dbContext)
    { }

    public async Task<AccountDto> HandleAsync(DepositAccountCommand command, CancellationToken cancellationToken)
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
        return AccountMapper.ToDto(account);
    }
}
