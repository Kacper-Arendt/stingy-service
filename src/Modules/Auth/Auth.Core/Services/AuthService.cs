using Auth.Core.Dtos;
using Auth.Core.Entities;
using Auth.Core.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Shared.Abstractions.Events;
using Shared.Abstractions.ValueObjects;

namespace Auth.Core.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly SignInManager<User> _signInManager;
    private readonly IEventPublisher _eventPublisher;

    public AuthService(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor,
        SignInManager<User> signInManager, IEventPublisher eventPublisher)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _signInManager = signInManager;
        _eventPublisher = eventPublisher;
    }

    public async Task<RegisterUserResponseDto> RegisterUser(RegisterUserDto registerUserDto)
    {
        var user = User.CreateNormalUser(registerUserDto.Email);
        var result = await _userManager.CreateAsync(user, registerUserDto.Password);

        if (!result.Succeeded)
        {
            throw new UserCreationFailedException(result.Errors.ToList());
        }

        await _eventPublisher.PublishAsync(new UserRegisteredEvent
        {
            UserId = new UserId(Guid.Parse(user.Id)),
            Email = new Email(registerUserDto.Email),
            RegisteredAt = DateTime.UtcNow
        });

        return new RegisterUserResponseDto(user.Id);
    }

    public async Task<CurrentUserResponseDto?> GetCurrentUser()
    {
        var name = _httpContextAccessor.HttpContext?.User.Identity?.Name;
        var user = await _userManager.FindByEmailAsync(name);
        var roles = await _userManager.GetRolesAsync(user);

        return name == null ? null : new CurrentUserResponseDto(name, user.Id, roles);
    }

    public async Task Logout()
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext == null || httpContext.User.Identity?.IsAuthenticated != true)
        {
            throw new UserNotLoggedInException();
        }

        await _signInManager.SignOutAsync();
        // httpContext.Response.Cookies.Delete(".AspNetCore.Identity.Application");
    }
}