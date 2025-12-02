using UserProfiles.Domain.Entities;
using Shared.Abstractions.ValueObjects;

namespace UserProfiles.Core.Permissions;

public interface IUserProfilePermissionService
{
    bool CanEditProfile(UserId currentUserId, UserId profileUserId);
    bool CanViewProfile(UserId? currentUserId, UserProfile userProfile);
    bool CanViewFullProfile(UserId? currentUserId, UserProfile userProfile);
}
