namespace EventSourcing.Data;

public class AggregateBaseEntity : BaseEntity
{
    public int Version { get; set; }
}
