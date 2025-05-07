using Microsoft.EntityFrameworkCore;
namespace EventSourcing.Data;

public class EventStoreDbContext : DbContext
{
    public DbSet<Event> Events { get; set; }

    public EventStoreDbContext(DbContextOptions<EventStoreDbContext> options)
    : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasPostgresExtension("uuid-ossp");

        builder.Entity<Event>(entity =>
        {
            entity
                .HasKey(e => e.Id);
            entity.Property(x => x.Id)
                .HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(e => e.AggregateId)
                .IsRequired();
            entity.Property(e => e.AggregateType)
                .IsRequired()
                .HasConversion<string>();
            entity.Property(e => e.EventType)
                .IsRequired()
                .HasConversion<string>();
            entity.Property(e => e.Version)
                .IsRequired();
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var insertedEntries = ChangeTracker.Entries()
                               .Where(x => x.State == EntityState.Added)
                               .Select(x => x.Entity);

        foreach (var insertedEntry in insertedEntries)
        {
            if (insertedEntry is BaseEntity baseEntity)
            {
                baseEntity.Created = DateTime.UtcNow;
            }
            else if (insertedEntry is BaseUpdatebleEntity baseUpdatebleEntity)
            {
                baseUpdatebleEntity.Created = DateTime.UtcNow;
                baseUpdatebleEntity.Updated = DateTime.UtcNow;
            }
        }

        var modifiedEntries = ChangeTracker.Entries()
                   .Where(x => x.State == EntityState.Modified)
                   .Select(x => x.Entity);

        foreach (var modifiedEntry in modifiedEntries)
        {
            if (modifiedEntry is BaseUpdatebleEntity baseEntity)
            {
                baseEntity.Updated = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
