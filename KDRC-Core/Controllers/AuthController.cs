using KDRC_Core.Models.Requests;
using KDRC_Core.Models.Responses;
using KDRC_Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace KDRC_Core.Controllers;

[ApiController]
[Route("/api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginRequest loginRequest)
    {
        // Do stuffs
        var result = await _authService.LoginAccountAsync(loginRequest);

        if (result == null)
        {
            return Unauthorized(new ErrorResponse
            {
                TraceId = HttpContext.TraceIdentifier,
                Message = "Please check your email or password!"
            });
        }

        return Ok(new LoginResponse
        {
            Token = result.Id,
            ValidUntil = result.ValidUntil
        });
    }
}