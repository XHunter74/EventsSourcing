using EventSourcing.Events.Account;
using EventSourcing.Events;

namespace EventSourcing.Aggregates;

public class AccountAggregate : AggregateRoot
{
    public string OwnerName { get; private set; }
    public decimal Balance { get; private set; }

    public void Apply(IEvent @event)
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

    //public static BankAccount Create(Guid id, string ownerName)
    //{
    //    var e = new AccountCreated(Guid.NewGuid(),id, ownerName, DateTime.UtcNow);
    //    var account = new BankAccount();
    //    account.Apply(e);
    //    account._uncommittedEvents.Add(e);
    //    return account;
    //}

    //public void Deposit(decimal amount)
    //{
    //    var e = new MoneyDeposited(Id, amount, DateTime.UtcNow);
    //    Apply(e);
    //    _uncommittedEvents.Add(e);
    //}

    //public void Withdraw(decimal amount)
    //{
    //    if (amount > Balance)
    //        throw new InvalidOperationException("Insufficient funds");
    //    var e = new MoneyWithdrawn(Id, amount, DateTime.UtcNow);
    //    Apply(e);
    //    _uncommittedEvents.Add(e);
    //}
}
