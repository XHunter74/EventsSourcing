namespace EventSourcing.Data;

public class Account : AggregateBaseEntity
{
    public Guid Id { get; set; }
    public string OwnerName { get; set; }
    public decimal Balance { get; set; }
    public int Version { get; set; }
}
