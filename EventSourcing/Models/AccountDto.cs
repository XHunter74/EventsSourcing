namespace EventSourcing.Models;

/// <summary>
/// Data Transfer Object (DTO) for Account.
/// </summary>
/// <param name="Id">Account Id</param>
/// <param name="OwnerName">Owner name</param>
/// <param name="Balance">Account balance</param>
/// <param name="version">Object version</param>
public record AccountDto(Guid Id, string OwnerName, decimal Balance, int version);
