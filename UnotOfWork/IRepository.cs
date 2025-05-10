namespace EventSourcing.UnotOfWork;

public interface IRepository<T> where T : class
{
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
}
