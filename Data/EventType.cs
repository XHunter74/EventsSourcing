using EventSourcing.Events;

namespace EventSourcing.Data;

public enum EventType
{
    #region BankAccount
    AccountCreated = 1,
    MoneyDeposited = 2,
    MoneyWithdrawn = 3
    #endregion
}
