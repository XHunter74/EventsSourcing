using CQRSMediatr.Interfaces;
using EventSourcing.Data;

namespace EventSourcing.Features.Commands;

public class CreateAccountCommand : ICommand<Guid>
{
    public string OwnerName { get; set; }
}

public class TestCommandHandler : ICommandHandler<CreateAccountCommand, Guid>
{
    private readonly EventStoreDbContext _dbContext;

    public TestCommandHandler(EventStoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

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
        await _dbContext.Events.AddAsync(newEvent, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return newEvent.AggregateId;
    }
}
