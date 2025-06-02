namespace EventSourcing.Data;

public class Event : BaseEntity
{
    public Guid Id { get; set; }
    public Guid AggregateId { get; set; }
    public AggregateType AggregateType { get; set; }
    public EventType EventType { get; set; }
    public decimal? DecimalData { get; set; }
    public int? IntData { get; set; }
    public string? StringData { get; set; }
    public bool? BooleanData { get; set; }
    public int Version { get; set; }
}