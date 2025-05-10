using EventSourcing.Events.Account;
using EventSourcing.Events;

namespace EventSourcing.Aggregates;

public class AccountAggregateNew : AggregateRootNew
{
    public string OwnerName { get; private set; }
    public decimal Balance { get; private set; }

    public static AccountAggregateNew Create(string ownerName)
    {
        var newAccount = new AccountAggregateNew();
        var accountCreatedEvent = new AccountCreated(Guid.NewGuid(), Guid.NewGuid(), ownerName, 1, DateTime.UtcNow);
        newAccount.LoadsFromHistory([accountCreatedEvent], true);
        return newAccount;
    }

    public override void Apply(IEvent @event)
    {
        switch (@event)
        {
            case AccountCreated e:
                Id = e.AccountId;
                OwnerName = e.OwnerName;
                Balance = 0;
                Version = e.Version;
                break;
            case MoneyDeposited e:
                Balance += e.Amount;
                Version = e.Version;
                break;
            case MoneyWithdrawn e:
                Balance -= e.Amount;
                Version = e.Version;
                break;
        }
    }
}
