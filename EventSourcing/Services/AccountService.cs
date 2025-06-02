using EventSourcing.Aggregates;
using EventSourcing.Data;
using EventSourcing.Exceptions;
using EventSourcing.Mappers;
using EventSourcing.Models;
using EventSourcing.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventSourcing.Services;

public class AccountService : IAccountService
{
    private readonly ILogger<AccountService> _logger;
    private readonly EventStoreDbContext _dbContext;
    private readonly IMessageBusService _messageBusService;

    public AccountService(ILogger<AccountService> logger,
        EventStoreDbContext dbContext,
        IMessageBusService messageBusService)
    {
        _logger = logger;
        _dbContext = dbContext;
        _messageBusService = messageBusService;
    }

    public async Task<Guid> CreateAccountAsync(string ownerName, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Called CreateAccountAsync with ownerName: {OwnerName}", ownerName);
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

        var logEvent= new LogMessageDto { 
            AggregateId = newEvent.AggregateId, 
            AggregateType = AggregateType.Account, 
            EventType = EventType.AccountCreated,
            Message = $"Account created for {ownerName}",
        };

        await _messageBusService.SendMessageToQueue(Constants.LogQueueName, logEvent);

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
        _logger.LogInformation("Called DepositAccountAsync with id: {Id}, amount: {Amount}", id, amount);
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

        var logEvent = new LogMessageDto
        {
            AggregateId = newEvent.AggregateId,
            AggregateType = AggregateType.Account,
            EventType = EventType.MoneyDeposited,
            Message = $"Account deposited for {amount}",
        };

        await _messageBusService.SendMessageToQueue(Constants.LogQueueName, logEvent);

        var domainEvent = EventsMapper.ToDomainEvent(newEvent);
        account.Apply(domainEvent);
        return account;
    }

    public async Task<AccountAggregate> WithdrawAccountAsync(Guid id, decimal amount, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Called WithdrawAccountAsync with id: {Id}, amount: {Amount}", id, amount);
        var account = await GetAccountAggregateAsync(id, null, cancellationToken);

        if (account.Balance < amount)
        {
            _logger.LogWarning("Insufficient balance for withdrawal. AccountId: {Id}, Balance: {Balance}, Attempted: {Amount}", id, account.Balance, amount);
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

        var logEvent = new LogMessageDto
        {
            AggregateId = newEvent.AggregateId,
            AggregateType = AggregateType.Account,
            EventType = EventType.MoneyWithdrawn,
            Message = $"Account withdrawn for {amount}",
        };

        await _messageBusService.SendMessageToQueue(Constants.LogQueueName, logEvent);

        var domainEvent = EventsMapper.ToDomainEvent(newEvent);
        account.Apply(domainEvent);
        return account;
    }

    public async Task<AccountAggregate> GetAccountByIdAsync(Guid id, int? version, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Called GetAccountByIdAsync with id: {Id}, version: {Version}", id, version);
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
        _logger.LogInformation("Called SaveAccountProjectionAsync with id: {Id}", id);
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

    public async Task<AccountAggregate[]> GetAllAccounts(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Called GetAllAccounts");
        var events = await _dbContext.Events
            .Where(e => e.AggregateType == AggregateType.Account && e.Version == 1)
            .OrderBy(e => e.Created)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var result = new List<AccountAggregate>();

        foreach (var e in events)
        {
            var account = new AccountAggregate();
            account.Apply(EventsMapper.ToDomainEvent(e));
            result.Add(account);
        }
        return result.ToArray();
    }
}
