using CQRSMediatr.Interfaces;
using EventSourcing.Data;

namespace EventSourcing.Features.Commands;

public class CreateAccountCommand : ICommand<Guid>
{
    public string OwnerName { get; set; }
}

public class CreateAccountHandler : BaseFeatureHandler, ICommandHandler<CreateAccountCommand, Guid>
{

    public CreateAccountHandler(ILogger<CreateAccountHandler> logger,
        EventStoreDbContext dbContext) : base(logger, dbContext)
    { }

    public async Task<Guid> HandleAsync(CreateAccountCommand command, CancellationToken cancellationToken)
    {
        var newEvent = new Event
        {
            AggregateId = Guid.NewGuid(),
            AggregateType = AggregateType.Account,
            EventType = EventType.AccountCreated,
            StringData = command.OwnerName,
            Version = 1
        };
        await DbContext.Events.AddAsync(newEvent, cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken);
        return newEvent.AggregateId;
    }
}
