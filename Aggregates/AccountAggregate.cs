﻿using EventSourcing.Events.Account;
using EventSourcing.Events;

namespace EventSourcing.Aggregates;

public class AccountAggregate : AggregateRoot
{
    public string OwnerName { get; private set; }
    public decimal Balance { get; private set; }

    public AccountAggregate() { }

    public AccountAggregate(Guid id, string ownerName, decimal balance, int version) :
        base(id, version)
    {
        OwnerName = ownerName;
        Balance = balance;
    }

    public override void Apply(IEvent @event)
    {
        switch (@event)
        {
            case AccountCreated e:
                Id = e.AccountId;
                OwnerName = e.OwnerName;
                Balance = 0;
                break;
            case MoneyDeposited e:
                Balance += e.Amount;
                break;
            case MoneyWithdrawn e:
                Balance -= e.Amount;
                break;
        }
    }
}
