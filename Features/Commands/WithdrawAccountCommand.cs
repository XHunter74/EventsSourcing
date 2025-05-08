using CQRSMediatr.Interfaces;
using EventSourcing.Data;
using EventSourcing.Exceptions;
using EventSourcing.Mappers;
using EventSourcing.Models;

namespace EventSourcing.Features.Commands;

public class WithdrawAccountCommand : ICommand<AccountDto>
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
}

public class WithdrawAccountCommandHandler : BaseAccountHandler, ICommandHandler<WithdrawAccountCommand, AccountDto>
{

    public WithdrawAccountCommandHandler(ILogger<WithdrawAccountCommandHandler> logger,
        EventStoreDbContext dbContext) : base(logger, dbContext)
    { }

    public async Task<AccountDto> HandleAsync(WithdrawAccountCommand command, CancellationToken cancellationToken)
    {
        var account = await GetAccountAggregateAsync(command.Id, null, cancellationToken);

        if (account.Balance < command.Amount)
        {
            throw new BadRequestException($"Insuficient account balans: {account.Balance}");
        }

        var newEvent = new Event
        {
            AggregateId = command.Id,
            AggregateType = AggregateType.Account,
            EventType = EventType.MoneyWithdrawn,
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
