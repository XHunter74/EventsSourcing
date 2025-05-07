using EventSourcing.Data;
using Microsoft.EntityFrameworkCore;

namespace EventSourcing.Extensions;

public static class DatabaseExtensions
{
    public static IHost ApplyDbMigrations(this IHost host)
    {
        using var serviceScope = host.Services.CreateScope();
        using var context = (DbContext)serviceScope.ServiceProvider.GetRequiredService<EventStoreDbContext>();

        context.Database.Migrate();

        return host;
    }
}
