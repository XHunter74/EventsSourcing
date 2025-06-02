using CQRSMediatr.Interfaces;
using EventSourcing.Aggregates;
using EventSourcing.Data;
using EventSourcing.Exceptions;
using EventSourcing.Mappers;
using EventSourcing.Models;
using EventSourcing.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventSourcing.Features.Commands;

public class SaveAccountProjectionCommand : ICommand<AccountDto>
{
    public Guid Id { get; set; }
}

public class SaveAccountProjectionHandler : BaseFeatureHandler, ICommandHandler<SaveAccountProjectionCommand, AccountDto>
{

    public SaveAccountProjectionHandler(ILogger<SaveAccountProjectionCommand> logger,
        IAccountService accountService) : base(logger, accountService)
    { }

    public async Task<AccountDto> HandleAsync(SaveAccountProjectionCommand command, CancellationToken cancellationToken)
    {
        var account = await AccountService.SaveAccountProjectionAsync(command.Id, cancellationToken);
        return AccountMapper.ToDto(account);
    }
}
