using EventSourcing.Data;

namespace EventSourcing.UnotOfWork;

public class AccountRepository : AggregateRepository<Account>
{
    public AccountRepository(EventStoreDbContext dbContext, IEventsRepository eventsRepository)
        : base(dbContext, eventsRepository)
    {
    }
}
