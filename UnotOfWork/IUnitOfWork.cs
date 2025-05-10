namespace EventSourcing.UnotOfWork;

public interface IUnitOfWork
{
    EventsRepository EventsRepository { get; }
    AccountRepository AccountRepository { get; }
}
