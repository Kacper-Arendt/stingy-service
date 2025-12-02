using UserProfiles.Core.Queries.Dtos;
using Shared.Abstractions.ValueObjects;

namespace UserProfiles.Core.Queries;

public interface IUserProfileQueryService
{
    Task<UserProfileDto?> GetByUserIdAsync(UserId userId);
    Task<PublicProfileDto?> GetPublicProfileAsync(UserId userId, UserId? viewerId = null);

}
