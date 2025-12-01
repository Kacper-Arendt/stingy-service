using Auth.Core.Dtos;
using Auth.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> RegisterUser(RegisterUserDto registerUserDto)
    {
        var createdUser = await _authService.RegisterUser(registerUserDto);

        return Ok(new { Id = createdUser.UserId });
    }
    
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var user = await _authService.GetCurrentUser();
        return Ok(user);
    }
    
    [HttpPost("Logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _authService.Logout();
        return NoContent();
    }
}