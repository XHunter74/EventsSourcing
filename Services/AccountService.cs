﻿using EventSourcing.Aggregates;
using EventSourcing.Data;
using EventSourcing.Exceptions;
using EventSourcing.Mappers;
using Microsoft.EntityFrameworkCore;

namespace EventSourcing.Services;

public class AccountService : IAccountService
{
    private readonly ILogger<AccountService> _logger;
    private readonly EventStoreDbContext _dbContext;
    public AccountService(ILogger<AccountService> logger, EventStoreDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<Guid> CreateAccountAsync(string ownerName, CancellationToken cancellationToken)
    {
        var newEvent = new Event
        {
            AggregateId = Guid.NewGuid(),
            AggregateType = AggregateType.Account,
            EventType = EventType.AccountCreated,
            StringData = ownerName,
            Version = 1
        };
        await _dbContext.Events.AddAsync(newEvent, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return newEvent.AggregateId;
    }

    private async Task<IEnumerable<Event>> GetAggregateEventsAsync(Guid aggregateId,
        int? version = 0, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Events
            .Where(e => e.AggregateId == aggregateId);
        if (version.HasValue)
        {
            query = query.Where(e => e.Version <= version);
        }
        var events = await query
            .OrderBy(e => e.Created)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        return events;
    }

    public async Task<AccountAggregate> DepositAccountAsync(Guid id, decimal amount, CancellationToken cancellationToken)
    {
        var account = await GetAccountAggregateAsync(id, null, cancellationToken);

        var newEvent = new Event
        {
            AggregateId = id,
            AggregateType = AggregateType.Account,
            EventType = EventType.MoneyDeposited,
            DecimalData = amount,
            Version = account.Version + 1
        };
        await _dbContext.Events.AddAsync(newEvent, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var domainEvent = EventsMapper.ToDomainEvent(newEvent);
        account.Apply(domainEvent);
        return account;
    }

    public async Task<AccountAggregate> WithdrawAccountAsync(Guid id, decimal amount, CancellationToken cancellationToken)
    {
        var account = await GetAccountAggregateAsync(id, null, cancellationToken);

        if (account.Balance < amount)
        {
            throw new BadRequestException($"Insuficient account balans: {account.Balance}");
        }

        var newEvent = new Event
        {
            AggregateId = id,
            AggregateType = AggregateType.Account,
            EventType = EventType.MoneyWithdrawn,
            DecimalData = amount,
            Version = account.Version + 1
        };
        await _dbContext.Events.AddAsync(newEvent, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var domainEvent = EventsMapper.ToDomainEvent(newEvent);
        account.Apply(domainEvent);
        return account;
    }

    public async Task<AccountAggregate> GetAccountByIdAsync(Guid id, int? version, CancellationToken cancellationToken)
    {
        var account = await GetAccountAggregateAsync(id, version, cancellationToken: cancellationToken);
        return account;
    }


    private async Task<AccountAggregate> GetAccountAggregateAsync(Guid Id, int? version,
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

        var accountFromDb = await _dbContext.Accounts
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

    public async Task<AccountAggregate> SaveAccountProjectionAsync(Guid id, CancellationToken cancellationToken)
    {
        var account = await GetAccountAggregateAsync(id, null, cancellationToken: cancellationToken);

        var newAccountRecord = new Account
        {
            Id = account.Id,
            OwnerName = account.OwnerName,
            Balance = account.Balance,
            Version = account.Version
        };
        await _dbContext.Accounts.AddAsync(newAccountRecord, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return account;
    }
}
