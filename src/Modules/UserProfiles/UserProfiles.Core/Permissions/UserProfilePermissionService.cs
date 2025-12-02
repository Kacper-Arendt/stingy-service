using UserProfiles.Domain.Entities;
using UserProfiles.Domain.Enums;
using Shared.Abstractions.ValueObjects;

namespace UserProfiles.Core.Permissions;

public class UserProfilePermissionService : IUserProfilePermissionService
{
    public bool CanEditProfile(UserId currentUserId, UserId profileUserId)
    {
        return currentUserId.Value == profileUserId.Value;
    }

    public bool CanViewProfile(UserId? currentUserId, UserProfile userProfile)
    {
        return userProfile.IsVisibleForUser(currentUserId);
    }

    public bool CanViewFullProfile(UserId? currentUserId, UserProfile userProfile)
    {
        return userProfile.CanViewFullProfile(currentUserId);
    }


}
