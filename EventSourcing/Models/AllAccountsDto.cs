namespace EventSourcing.Models;


public record AccountInfoDto(Guid Id, string OwnerName);

public class AllAccountsDto
{
    public AccountInfoDto[] Accounts { get; set; } = [];
    public int TotalCount { get; set; }
    public AllAccountsDto() { }
    public AllAccountsDto(AccountInfoDto[] accounts, int totalCount)
    {
        Accounts = accounts;
        TotalCount = totalCount;
    }
}
