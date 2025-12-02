using UserProfiles.Domain.Entities;
using UserProfiles.Domain.ValueObjects;
using Shared.Abstractions.ValueObjects;

namespace UserProfiles.Core.Repositories;

public interface IUserProfileRepository
{
    Task<UserProfile?> GetByUserIdAsync(UserId userId);
    Task<UserProfile?> GetByDisplayNameAsync(DisplayName displayName);
    Task<bool> ExistsAsync(UserId userId);
    Task<bool> IsDisplayNameTakenAsync(DisplayName displayName, UserId? excludeUserId = null);
    Task SaveAsync(UserProfile userProfile);
    Task UpdateAsync(UserProfile userProfile);
}
