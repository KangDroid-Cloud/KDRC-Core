using KDRC_Core.Models.Requests;
using KDRC_Core.Models.Responses;
using KDRC_Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace KDRC_Core.Controllers;

[ApiController]
[Route("/api/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Login to KDR-Cloud
    /// </summary>
    /// <param name="loginRequest">Login Request Body(Model)</param>
    /// <returns></returns>
    /// <response code = "200">When successfully logged-in to KDR-Cloud</response>
    /// <response code = "401">When login failed</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
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