using EventSourcing.Data;
using Microsoft.EntityFrameworkCore;

namespace EventSourcing.UnotOfWork;

public class EventsRepository : IEventsRepository, IRepository<Event>
{
    private readonly EventStoreDbContext _dbContext;
    public EventsRepository(EventStoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<Event> AddAsync(Event @event, CancellationToken cancellationToken = default)
    {
        await _dbContext.Events.AddAsync(@event, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return @event;
    }
    public async Task<IEnumerable<Event>> GetEventsAsync(Guid aggregateId,
        int? version = 0, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Events
            .Where(e => e.AggregateId == aggregateId);
        if (version.HasValue)
        {
            query = query.Where(e => e.Version <= version);
        }
        var events = await query
            .OrderBy(e => e.Created)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        return events;
    }
}
