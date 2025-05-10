using EventSourcing.Data;

namespace EventSourcing.UnotOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly EventStoreDbContext _dbContext;
    private EventsRepository _eventsRepository;
    private AccountRepository _accountRepository;

    public EventsRepository EventsRepository =>
        _eventsRepository ??= new EventsRepository(_dbContext);
    public AccountRepository AccountRepository =>
        _accountRepository ??= new AccountRepository(_dbContext, EventsRepository);

    public UnitOfWork(EventStoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }
}
