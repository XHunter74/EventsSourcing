using EventSourcing.Aggregates;
using EventSourcing.Data;
using EventSourcing.Exceptions;
using EventSourcing.Mappers;
using Microsoft.EntityFrameworkCore;

namespace EventSourcing.Features;

public class BaseAccountHandler : BaseFeatureHandler
{
    public BaseAccountHandler(ILogger logger, EventStoreDbContext dbContext) : base(logger, dbContext)
    { }

    public async Task<AccountAggregate> GetAccountAggregateAsync(Guid Id, int? version,
        CancellationToken cancellationToken = default)
    {
        var events = await GetAggregateEventsAsync(Id, version, cancellationToken);

        if (!events.Any())
        {
            throw new NotFoundException($"Account with id {Id} not found.");
        }
        var maxVersion = events.Max(e => e.Version);
        version ??= maxVersion;

        if (version < 0 || version > maxVersion)
        {
            throw new BadRequestException($"Incorrect account version: '{version}'");
        }

        var accountFromDb = await DbContext.Accounts
            .AsNoTracking()
            .Where(a => a.Id == Id && a.Version <= version)
            .OrderByDescending(a => a.Version)
            .FirstOrDefaultAsync(cancellationToken);

        if (accountFromDb?.Version == version)
        {
            return new AccountAggregate(accountFromDb.Id, accountFromDb.OwnerName,
                accountFromDb.Balance, accountFromDb.Version);
        }

        var account = new AccountAggregate();
        if (accountFromDb != null)
        {
            events = events
                .Where(e => e.Version > accountFromDb.Version)
                .ToList();
            account = new AccountAggregate(accountFromDb.Id, accountFromDb.OwnerName,
                accountFromDb.Balance, accountFromDb.Version);
        }

        account.LoadsFromHistory(events.Select(EventsMapper.ToDomainEvent));
        return account;
    }
}
