using CQRSMediatr.Interfaces;
using EventSourcing.Mappers;
using EventSourcing.Models;
using EventSourcing.Services;

namespace EventSourcing.Features.Queries;

public class GetAccountByIdQuery : IQuery<AccountDto>
{
    public Guid Id { get; set; }
    public int? Version { get; set; }
}

public class GetAccountByIdQueryHandler : BaseFeatureHandler, IQueryHandler<GetAccountByIdQuery, AccountDto>
{
    public GetAccountByIdQueryHandler(ILogger<GetAccountByIdQueryHandler> logger,
        IAccountService accountService) : base(logger, accountService)
    { }

    public async Task<AccountDto> HandleAsync(GetAccountByIdQuery query, CancellationToken cancellationToken)
    {
        var account = await AccountService.GetAccountByIdAsync(query.Id, query.Version, cancellationToken);
        return AccountMapper.ToDto(account);
    }
}
