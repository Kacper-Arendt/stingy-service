using UserProfiles.Domain.Enums;

namespace UserProfiles.Core.Queries.Dtos;

public class PublicProfileDto
{
    public Guid UserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public string? Bio { get; set; }
    public ProfileVisibilityLevel Visibility { get; set; }
    public DateTime CreatedAt { get; set; }
}
