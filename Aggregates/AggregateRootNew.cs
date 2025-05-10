using EventSourcing.Events;

namespace EventSourcing.Aggregates;

public abstract class AggregateRootNew
{
    private readonly List<IEvent> _uncommittedEvents = new();
    public Guid Id { get; protected set; }
    public int Version { get; protected set; }

    public IReadOnlyCollection<IEvent> GetUncommittedEvents() => _uncommittedEvents.AsReadOnly();

    public void ClearUncommittedEvents() => _uncommittedEvents.Clear();

    protected AggregateRootNew() { }

    public abstract void Apply(IEvent @event);

    public void LoadsFromHistory(IEnumerable<IEvent> history, bool isNew = false)
    {
        foreach (var e in history.Where(e => e.Version > Version))
        {
            Apply(e);
            if (isNew)
            {
                _uncommittedEvents.Add(e);
            }
        }
    }
}