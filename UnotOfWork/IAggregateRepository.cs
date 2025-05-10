namespace EventSourcing.UnotOfWork;

public interface IAggregateRepository<T> where T : class
{
    Task<T> GetAsync(Guid Id, int? version,
        CancellationToken cancellationToken = default);
}
