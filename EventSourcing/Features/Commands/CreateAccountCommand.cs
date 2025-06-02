using CQRSMediatr.Interfaces;
using EventSourcing.Services;

namespace EventSourcing.Features.Commands;

public class CreateAccountCommand : ICommand<Guid>
{
    public string OwnerName { get; set; }
}

public class CreateAccountHandler : BaseFeatureHandler, ICommandHandler<CreateAccountCommand, Guid>
{

    public CreateAccountHandler(ILogger<CreateAccountHandler> logger,
        IAccountService accountService) : base(logger, accountService)
    { }

    public async Task<Guid> HandleAsync(CreateAccountCommand command, CancellationToken cancellationToken)
    {
        return await AccountService.CreateAccountAsync(command.OwnerName, cancellationToken);
    }
}
