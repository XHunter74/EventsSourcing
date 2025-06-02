using CQRSMediatr.Interfaces;
using EventSourcing.Exceptions;
using EventSourcing.Features.Commands;
using EventSourcing.Features.Queries;
using EventSourcing.Models;
using Microsoft.AspNetCore.Mvc;

namespace EventSourcing.Controllers;

/// <summary>
/// Controller for managing accounts.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly ICqrsMediatr _mediatr;

    public AccountController(ICqrsMediatr mediatr)
    {
        _mediatr = mediatr;
    }


    /// <summary>
    /// Retrieves all accounts.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>Returns a list of all accounts.</returns>
    /// <response code="200">Returns the list of accounts.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AccountDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAccountsAsync(CancellationToken cancellationToken)
    {
        var query = new GetAllAccountsQuery();
        var result = await _mediatr.QueryAsync(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get accounts by Id.
    /// </summary>
    /// <param name="id">Account Id</param>
    /// <param name="version">Account version</param>
    /// <param name="cancellationToken">Cancelation token</param>
    /// <returns>Returns the account details if found, otherwise a 404 error.</returns>
    /// <response code="200">Returns the account details.</response>
    /// <response code="404">If the account is not found.</response>
    [HttpGet("{id:guid}", Name = "GetAccountById")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AppExceptionModel), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAccountAsync(Guid id, int? version, CancellationToken cancellationToken)
    {
        var query = new GetAccountByIdQuery { Id = id, Version = version };
        var result = await _mediatr.QueryAsync(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Deposit money into an account.
    /// </summary>
    /// <param name="id">Account Id</param>
    /// <param name="amount">Money amount</param>
    /// <param name="cancellationToken">Cancelation Token</param>
    /// <returns></returns>
    [HttpPut("{id:guid}/deposit")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AppExceptionModel), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DepositAccountAsync(Guid id, decimal amount, CancellationToken cancellationToken)
    {
        var command = new DepositAccountCommand { Id = id, Amount = amount };
        var result = await _mediatr.SendAsync(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Withdraw money from an account.
    /// </summary>
    /// <param name="id">Account Id</param>
    /// <param name="amount">Money amount</param>
    /// <param name="cancellationToken">Cancelation Token</param>
    /// <returns></returns>
    [HttpPut("{id:guid}/withdraw")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AppExceptionModel), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> WithdrawAccountAsync(Guid id, decimal amount, CancellationToken cancellationToken)
    {
        var command = new WithdrawAccountCommand { Id = id, Amount = amount };
        var result = await _mediatr.SendAsync(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Create a new bank account.
    /// </summary>
    /// <param name="ownerName">Account owner name</param>
    /// <param name="cancellationToken">Cancelation token</param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(AppExceptionModel), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateNewAccountAsync(string ownerName, CancellationToken cancellationToken)
    {
        var command = new CreateAccountCommand
        {
            OwnerName = ownerName,
        };
        var accountId = await _mediatr.SendAsync(command, cancellationToken);
        return CreatedAtRoute("GetAccountById", new { id = accountId }, accountId);
    }

    /// <summary>
    /// Save the account projection.
    /// </summary>
    /// <param name="id">Account Id</param>
    /// <param name="cancellationToken">Cancelation Token</param>
    /// <returns></returns>
    [HttpPost("{id:guid}/save")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(AppExceptionModel), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SaveAccountProjectionAsync(Guid id, CancellationToken cancellationToken)
    {
        var command = new SaveAccountProjectionCommand { Id = id };
        var result = await _mediatr.SendAsync(command, cancellationToken);
        return CreatedAtRoute("GetAccountById", new { id = result.Id }, result);
    }
}
