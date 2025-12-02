using UserProfiles.Domain.Enums;

namespace UserProfiles.Core.Queries.Dtos;

public class UserProfileDto
{
    public Guid UserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public string? Bio { get; set; }
    public string TimeZone { get; set; } = string.Empty;
    public ProfileVisibilityLevel Visibility { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastModifiedAt { get; set; }
}
