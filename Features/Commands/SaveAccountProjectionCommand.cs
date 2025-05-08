using CQRSMediatr.Interfaces;
using EventSourcing.Aggregates;
using EventSourcing.Data;
using EventSourcing.Exceptions;
using EventSourcing.Mappers;
using EventSourcing.Models;
using Microsoft.EntityFrameworkCore;

namespace EventSourcing.Features.Commands;

public class SaveAccountProjectionCommand : ICommand<AccountDto>
{
    public Guid Id { get; set; }
}

public class SaveAccountProjectionHandler : BaseAccountHandler, ICommandHandler<SaveAccountProjectionCommand, AccountDto>
{

    public SaveAccountProjectionHandler(ILogger<SaveAccountProjectionCommand> logger,
        EventStoreDbContext dbContext) : base(logger, dbContext)
    { }

    public async Task<AccountDto> HandleAsync(SaveAccountProjectionCommand command, CancellationToken cancellationToken)
    {
        var account = await GetAccountAggregateAsync(command.Id, null, cancellationToken: cancellationToken);

        var newAccountRecord = new Account
        {
            Id = account.Id,
            OwnerName = account.OwnerName,
            Balance = account.Balance,
            Version = account.Version
        };
        await DbContext.Accounts.AddAsync(newAccountRecord, cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken);

        return AccountMapper.ToDto(account);
    }
}
