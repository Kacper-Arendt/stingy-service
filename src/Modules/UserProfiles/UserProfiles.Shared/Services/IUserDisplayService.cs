using UserProfiles.Shared.Dtos;
using Shared.Abstractions.ValueObjects;

namespace UserProfiles.Shared.Services;

public interface IUserDisplayService
{
    Task<UserDisplayInfoDto?> GetUserDisplayInfoAsync(UserId userId);
    Task<List<UserDisplayInfoDto>> GetUsersDisplayInfoAsync(IEnumerable<UserId> userIds);
}
