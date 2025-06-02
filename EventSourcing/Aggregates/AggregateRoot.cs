using EventSourcing.Events;

namespace EventSourcing.Aggregates;

public abstract class AggregateRoot
{
    public Guid Id { get; protected set; }
    public int Version { get; protected set; }

    protected AggregateRoot() { }

    protected AggregateRoot(Guid id, int version)
    {
        Id = id;
        Version = version;
    }

    public abstract void Apply(IEvent @event);

    public void LoadsFromHistory(IEnumerable<IEvent> history)
    {
        foreach (var e in history.Where(e => e.Version > Version))
            if (e.Version > Version)
            {
                Version = e.Version;
                Apply(e);
            }
    }
}