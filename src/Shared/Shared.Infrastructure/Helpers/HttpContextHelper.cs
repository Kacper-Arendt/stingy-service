using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Shared.Abstractions.ValueObjects;

namespace Shared.Infrastructure.Helpers;

public record CurrentUserDetails
{
    public UserId Id { get; init; }
    public Email Email { get; init; }
}

public class HttpContextHelper
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextHelper(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public CurrentUserDetails GetCurrentUser()
    {
        var ownerId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
        var email = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;

        if (Guid.TryParse(ownerId?.Value, out var guid))
        {
            return new CurrentUserDetails
            {
                Id = new UserId(guid),
                Email = new Email(email)
            };
        }

        throw new UnauthorizedAccessException("User is not authenticated");
    }
}