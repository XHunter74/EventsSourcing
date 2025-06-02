using EventSourcing.Aggregates;

namespace EventSourcing.Services.Interfaces;

public interface IAccountService
{
    Task<Guid> CreateAccountAsync(string ownerName, CancellationToken cancellationToken);
    Task<AccountAggregate> DepositAccountAsync(Guid id, decimal amount, CancellationToken cancellationToken);
    Task<AccountAggregate> GetAccountByIdAsync(Guid id, int? version, CancellationToken cancellationToken);
    Task<AccountAggregate> SaveAccountProjectionAsync(Guid id, CancellationToken cancellationToken);
    Task<AccountAggregate> WithdrawAccountAsync(Guid id, decimal amount, CancellationToken cancellationToken);
    Task<AccountAggregate[]> GetAllAccounts(CancellationToken cancellationToken);
}
