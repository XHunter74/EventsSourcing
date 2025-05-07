using CQRSMediatr.Interfaces;
using EventSourcing.Aggregates;
using EventSourcing.Data;
using EventSourcing.Mappers;
using Microsoft.EntityFrameworkCore;

namespace EventSourcing.Features.Queries;

public class GetAccountByIdQuery : IQuery<AccountAggregate>
{
    public Guid Id { get; set; }
}

public class GetAccountByIdQueryHandler : IQueryHandler<GetAccountByIdQuery, AccountAggregate>
{
    private readonly EventStoreDbContext _dbContext;

    public GetAccountByIdQueryHandler(EventStoreDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AccountAggregate> HandleAsync(GetAccountByIdQuery query, CancellationToken cancellationToken)
    {
        var events = (await _dbContext.Events
            .Where(e => e.AggregateId == query.Id)
            .OrderBy(e => e.Created)
            .AsNoTracking()
            .ToListAsync(cancellationToken))
            .Select(EventsMapper.ToDomainEvent)
            .ToList();
        var account = new AccountAggregate();
        account.LoadsFromHistory(events);
        return account;
    }
}
