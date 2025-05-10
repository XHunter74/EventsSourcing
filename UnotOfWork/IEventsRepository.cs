using EventSourcing.Data;

namespace EventSourcing.UnotOfWork;

public interface IEventsRepository
{
    Task<IEnumerable<Event>> GetEventsAsync(Guid aggregateId,
        int? version = 0, CancellationToken cancellationToken = default);
}
