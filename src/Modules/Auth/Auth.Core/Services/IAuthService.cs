using Auth.Core.Dtos;

namespace Auth.Core.Services;

public interface IAuthService
{
    Task<RegisterUserResponseDto> RegisterUser(RegisterUserDto registerUserDto);
    Task<CurrentUserResponseDto?> GetCurrentUser();
    Task Logout();
}