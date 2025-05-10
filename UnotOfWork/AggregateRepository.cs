using EventSourcing.Data;
using EventSourcing.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace EventSourcing.UnotOfWork;

public abstract class AggregateRepository<T> : IAggregateRepository<T> where T : AggregateBaseEntity
{
    private readonly EventStoreDbContext _dbContext;
    private readonly IEventsRepository _eventsRepository;

    public AggregateRepository(EventStoreDbContext dbContext, IEventsRepository eventsRepository)
    {
        _dbContext = dbContext;
        _eventsRepository = eventsRepository;
    }
    public async Task<T> GetAsync(Guid Id, int? version, CancellationToken cancellationToken = default)
    {
        var events = await _eventsRepository.GetEventsAsync(Id, version, cancellationToken);

        if (!events.Any())
        {
            throw new NotFoundException($"{typeof(T).Name} with id {Id} not found.");
        }
        var maxVersion = events.Max(e => e.Version);
        version ??= maxVersion;

        if (version < 0 || version > maxVersion)
        {
            throw new BadRequestException($"Incorrect entity version: '{version}'");
        }

        var entityFromDb = await _dbContext.Set<T>()
            .AsNoTracking()
            .Where(a => a.Id == Id && a.Version <= version)
            .OrderByDescending(a => a.Version)
            .FirstOrDefaultAsync(cancellationToken);

        return entityFromDb ?? throw new NotFoundException($"{typeof(T).Name} with id {Id} not found.");
    }
}
