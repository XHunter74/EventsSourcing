using EventSourcing.Aggregates;
using EventSourcing.Data;
using EventSourcing.Models;

namespace EventSourcing.Mappers;

public static class AccountMapper
{
    public static AccountDto ToDto(AccountAggregate account)
    {
        return new AccountDto(account.Id, account.OwnerName, account.Balance, account.Version);
    }

    public static AccountDto ToDto(Account account)
    {
        return new AccountDto(account.Id, account.OwnerName, account.Balance, account.Version);
    }
}
