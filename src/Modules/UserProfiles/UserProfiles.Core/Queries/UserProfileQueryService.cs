using UserProfiles.Core.Queries.Dtos;
using UserProfiles.Core.Repositories;

using UserProfiles.Domain.Entities;
using Shared.Abstractions.ValueObjects;

namespace UserProfiles.Core.Queries;

public class UserProfileQueryService : IUserProfileQueryService
{
    private readonly IUserProfileRepository _userProfileRepository;
    public UserProfileQueryService(IUserProfileRepository userProfileRepository)
    {
        _userProfileRepository = userProfileRepository;
    }

    public async Task<UserProfileDto?> GetByUserIdAsync(UserId userId)
    {
        var userProfile = await _userProfileRepository.GetByUserIdAsync(userId);
        return userProfile == null ? null : MapToDto(userProfile);
    }

    public async Task<PublicProfileDto?> GetPublicProfileAsync(UserId userId, UserId? viewerId = null)
    {
        var userProfile = await _userProfileRepository.GetByUserIdAsync(userId);
        if (userProfile == null)
            return null;

        if (!userProfile.IsVisibleForUser(viewerId))
            return null;

        return MapToPublicDto(userProfile, viewerId);
    }







    private static UserProfileDto MapToDto(UserProfile userProfile)
    {
        return new UserProfileDto
        {
            UserId = userProfile.UserId.Value,
            DisplayName = userProfile.DisplayName.Value,
            ProfileImageUrl = userProfile.ProfileImageUrl?.Value,
            Bio = userProfile.Bio?.Value,
            TimeZone = userProfile.TimeZone.Value,
            Visibility = userProfile.Visibility,
            CreatedAt = userProfile.CreatedAt.Value,
            LastModifiedAt = userProfile.LastModifiedAt.Value
        };
    }

    private static PublicProfileDto MapToPublicDto(UserProfile userProfile, UserId? viewerId)
    {
        var canViewFullProfile = userProfile.CanViewFullProfile(viewerId);

        return new PublicProfileDto
        {
            UserId = userProfile.UserId.Value,
            DisplayName = userProfile.DisplayName.Value,
            ProfileImageUrl = userProfile.ProfileImageUrl?.Value,
            Bio = canViewFullProfile ? userProfile.Bio?.Value : null,
            Visibility = userProfile.Visibility,
            CreatedAt = userProfile.CreatedAt.Value
        };
    }
}
