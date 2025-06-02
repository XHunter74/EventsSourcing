using CQRSMediatr.Interfaces;
using EventSourcing.Mappers;
using EventSourcing.Models;
using EventSourcing.Services.Interfaces;

namespace EventSourcing.Features.Commands;

public class WithdrawAccountCommand : ICommand<AccountDto>
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
}

public class WithdrawAccountCommandHandler : BaseFeatureHandler, ICommandHandler<WithdrawAccountCommand, AccountDto>
{

    public WithdrawAccountCommandHandler(ILogger<WithdrawAccountCommandHandler> logger,
        IAccountService accountService) : base(logger, accountService)
    { }

    public async Task<AccountDto> HandleAsync(WithdrawAccountCommand command, CancellationToken cancellationToken)
    {
        var account = await AccountService.WithdrawAccountAsync(command.Id, command.Amount, cancellationToken);
        return AccountMapper.ToDto(account);
    }
}
