using Microsoft.AspNetCore.Mvc;
using TaskManagement.Dtos.Auth;
using TaskManagement.Services.Interfaces;

namespace TaskManagement.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/[controller]")]

public class AuthController : AppController
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(RegisterDto registerDto, CancellationToken ct)
    {
        var result = await _authService.RegisterAsync(registerDto, ct);
        if (!result.IsSuccess)
            return HandleError(result);

        return Created();
    }
    [HttpPost("login")]
    [ProducesResponseType(typeof(JwtDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login(LoginDto loginDto, CancellationToken ct)
    {
        var result = await _authService.LoginAsync(loginDto, ct);
        if (!result.IsSuccess)
            return HandleError(result);

        return Ok(result.Data);
    }
}
