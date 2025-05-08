namespace EventSourcing.Models;

public record AccountDto(Guid Id, string OwnerName, decimal Balance, int version);
