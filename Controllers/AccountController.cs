using CQRSMediatr.Interfaces;
using EventSourcing.Data;
using EventSourcing.Features.Commands;
using EventSourcing.Features.Queries;
using Microsoft.AspNetCore.Mvc;

namespace EventSourcing.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly ICqrsMediatr _mediatr;

    public AccountController(ICqrsMediatr mediatr)
    {
        _mediatr = mediatr;
    }

    [HttpGet("{id:guid}", Name = "GetAccountById")]
    public async Task<IActionResult> GetAccountAsync(Guid id, int? version, CancellationToken cancellationToken)
    {
        var query = new GetAccountByIdQuery { Id = id, Version = version };
        var result = await _mediatr.QueryAsync(query, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:guid}/deposit")]
    public async Task<IActionResult> DepositAccountAsync(Guid id, decimal amount, CancellationToken cancellationToken)
    {
        var command = new DepositAccountCommand { Id = id, Amount = amount };
        var result = await _mediatr.SendAsync(command, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:guid}/withdraw")]
    public async Task<IActionResult> WithdrawAccountAsync(Guid id, decimal amount, CancellationToken cancellationToken)
    {
        var command = new WithdrawAccountCommand { Id = id, Amount = amount };
        var result = await _mediatr.SendAsync(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateNewAccountAsync(CreateAccountCommand command, CancellationToken cancellationToken)
    {
        var accountId = await _mediatr.SendAsync(command, cancellationToken);
        return CreatedAtRoute("GetAccountById", new { id = accountId }, accountId);
    }

    [HttpPost("{id:guid}/save")]
    public async Task<IActionResult> SaveAccountProjectionAsync(Guid id, CancellationToken cancellationToken)
    {
        var command = new SaveAccountProjectionCommand { Id = id };
        var result = await _mediatr.SendAsync(command, cancellationToken);
        return CreatedAtRoute("GetAccountById", new { id = result.Id }, result);
    }
}
