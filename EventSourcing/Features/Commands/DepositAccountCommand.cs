using CQRSMediatr.Interfaces;
using EventSourcing.Mappers;
using EventSourcing.Models;
using EventSourcing.Services;

namespace EventSourcing.Features.Commands;

public class DepositAccountCommand : ICommand<AccountDto>
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
}

public class DepositAccountCommandHandler : BaseFeatureHandler, ICommandHandler<DepositAccountCommand, AccountDto>
{

    public DepositAccountCommandHandler(ILogger<DepositAccountCommandHandler> logger,
        IAccountService accountService) : base(logger, accountService)
    { }

    public async Task<AccountDto> HandleAsync(DepositAccountCommand command, CancellationToken cancellationToken)
    {
        var account = await AccountService.DepositAccountAsync(command.Id, command.Amount, cancellationToken);
        return AccountMapper.ToDto(account);
    }
}
