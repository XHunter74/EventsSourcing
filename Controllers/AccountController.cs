using CQRSMediatr.Interfaces;
using EventSourcing.Features.Commands;
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
        //var account = await _mediatr.SendAsync(new GetAccountQuery(id));
        //if (account == null)
        //    return NotFound();
        //return Ok(account);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> CreateNewAccountAsync(CreateAccountCommand command)
    {
        var accountId = await _mediatr.SendAsync(command);
        return CreatedAtRoute("GetAccountById", new { id = accountId }, accountId);
    }
}
