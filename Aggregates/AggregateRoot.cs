using EventSourcing.Events;

namespace EventSourcing.Aggregates;

public abstract class AggregateRoot
{
    private readonly List<IEvent> _uncommittedEvents = new();
    public Guid Id { get; protected set; }
    public int Version { get; protected set; }

    public IReadOnlyCollection<IEvent> GetUncommittedEvents() => _uncommittedEvents.AsReadOnly();

    public void ClearUncommittedEvents() => _uncommittedEvents.Clear();

    protected AggregateRoot() { }

    protected AggregateRoot(Guid id, int version)
    {
        Id = id;
        Version = version;
    }

    protected void ApplyChange(IEvent @event, bool isNew = true)
    {
        var method = GetType().GetMethod("Apply", new[] { @event.GetType() });
        if (method != null)
        {
            method.Invoke(this, new object[] { @event });
            if (isNew)
                _uncommittedEvents.Add(@event);
        }
    }

    public void LoadsFromHistory(IEnumerable<IEvent> history)
    {
        foreach (var e in history.Where(e => e.Version > Version))
            ApplyChange(e, false);
    }
}