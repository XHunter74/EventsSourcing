using EventSourcing.Data;
using Microsoft.EntityFrameworkCore;

namespace EventSourcing.Features;

public abstract class BaseFeatureHandler
{
    public BaseFeatureHandler(ILogger logger, EventStoreDbContext dbContext)
    {
        Logger = logger;
        DbContext = dbContext;
    }

    public ILogger Logger { get; }
    public EventStoreDbContext DbContext { get; }

    public async Task<IEnumerable<Event>> GetAggregateEventsAsync(Guid aggregateId,
        int? version = 0, CancellationToken cancellationToken = default)
    {
        var query = DbContext.Events
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
