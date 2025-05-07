using CQRSMediatr.Interfaces;
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
    public async Task<IActionResult> GetAccountAsync(Guid id)
    {
        var query = new GetAccountByIdQuery { Id = id };
        var result = await _mediatr.QueryAsync(query);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateNewAccountAsync(CreateAccountCommand command)
    {
        var accountId = await _mediatr.SendAsync(command);
        return CreatedAtRoute("GetAccountById", new { id = accountId }, accountId);
    }
}
