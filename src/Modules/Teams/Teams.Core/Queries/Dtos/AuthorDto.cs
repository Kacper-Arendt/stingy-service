using UserProfiles.Shared.Dtos;

namespace Teams.Core.Queries.Dtos;

public class AuthorDto
{
    public Guid UserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string ShortDisplayName { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }

    public static AuthorDto FromUserDisplayInfo(UserDisplayInfoDto userDisplayInfo)
    {
        return new AuthorDto
        {
            UserId = userDisplayInfo.UserId,
            DisplayName = userDisplayInfo.DisplayName,
            ShortDisplayName = userDisplayInfo.ShortDisplayName,
            ProfileImageUrl = userDisplayInfo.ProfileImageUrl
        };
    }
}
