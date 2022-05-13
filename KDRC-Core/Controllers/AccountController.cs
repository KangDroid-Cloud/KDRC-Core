using KDRC_Core.Models.Requests;
using KDRC_Core.Models.Responses;
using KDRC_Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace KDRC_Core.Controllers;

[ApiController]
[Route("/api/account")]
public class AccountController : ControllerBase
{
    private readonly AccountService _accountService;

    public AccountController(AccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> CreateAccountAsync(AccountRegisterRequest registerRequest)
    {
        if (!registerRequest.ValidateModel())
        {
            return BadRequest(new ErrorResponse
            {
                TraceId = HttpContext.TraceIdentifier,
                Message =
                    "Model validation failed. Please check your email address is valid and password length is at least 8 letters or more."
            });
        }

        var createdResult = await _accountService.CreateAccountAsync(registerRequest);
        if (createdResult == null)
        {
            return Conflict(new ErrorResponse
            {
                TraceId = HttpContext.TraceIdentifier,
                Message = $"User email with {registerRequest.Email} already exists!"
            });
        }

        return Ok();
    }
}