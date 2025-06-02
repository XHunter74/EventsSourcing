using CQRSMediatr.Interfaces;
using EventSourcing.Models;
using EventSourcing.Services;

namespace EventSourcing.Features.Queries;

public class GetAllAccountsQuery : IQuery<AllAccountsDto>
{
}

public class GetAllAccountsQueryHandler : BaseFeatureHandler, IQueryHandler<GetAllAccountsQuery, AllAccountsDto>
{
    public GetAllAccountsQueryHandler(ILogger<GetAccountByIdQueryHandler> logger,
        IAccountService accountService) : base(logger, accountService)
    { }

    public async Task<AllAccountsDto> HandleAsync(GetAllAccountsQuery query, CancellationToken cancellationToken)
    {
        var result = new AllAccountsDto();
        var accounts = await AccountService.GetAllAccounts(cancellationToken);
        result.Accounts = accounts
            .Select(e => new AccountInfoDto(e.Id, e.OwnerName))
            .ToArray();
        result.TotalCount = result.Accounts.Length;
        return result;
    }
}
