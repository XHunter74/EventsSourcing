using CQRSMediatr.Interfaces;
using EventSourcing.Data;
using EventSourcing.Mappers;
using EventSourcing.Models;

namespace EventSourcing.Features.Queries;

public class GetAccountByIdQuery : IQuery<AccountDto>
{
    public Guid Id { get; set; }
    public int? Version { get; set; }
}

public class GetAccountByIdQueryHandler : BaseAccountHandler, IQueryHandler<GetAccountByIdQuery, AccountDto>
{
    public GetAccountByIdQueryHandler(ILogger<GetAccountByIdQueryHandler> logger,
        EventStoreDbContext dbContext) : base(logger, dbContext)
    { }

    public async Task<AccountDto> HandleAsync(GetAccountByIdQuery query, CancellationToken cancellationToken)
    {
        var account = await GetAccountAggregateAsync(query.Id, query.Version, cancellationToken: cancellationToken);
        return AccountMapper.ToDto(account);
    }
}
