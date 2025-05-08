using CQRSMediatr.Interfaces;
using EventSourcing.Aggregates;
using EventSourcing.Data;
using EventSourcing.Mappers;

namespace EventSourcing.Features.Queries;

public class GetAccountByIdQuery : IQuery<AccountAggregate>
{
    public Guid Id { get; set; }
}

public class GetAccountByIdQueryHandler : BaseFeatureHandler, IQueryHandler<GetAccountByIdQuery, AccountAggregate>
{
    public GetAccountByIdQueryHandler(ILogger<GetAccountByIdQueryHandler> logger,
        EventStoreDbContext dbContext) : base(logger, dbContext)
    { }

    public async Task<AccountAggregate> HandleAsync(GetAccountByIdQuery query, CancellationToken cancellationToken)
    {
        var events = (await GetAggregateEvents(query.Id, cancellationToken: cancellationToken))
            .Select(EventsMapper.ToDomainEvent);
        var account = new AccountAggregate();
        account.LoadsFromHistory(events);
        return account;
    }
}
