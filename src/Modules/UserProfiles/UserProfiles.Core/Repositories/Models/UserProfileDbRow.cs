namespace UserProfiles.Core.Repositories.Models;

public class UserProfileDbRow
{
    public string UserId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public string? Bio { get; set; }
    public string TimeZone { get; set; } = string.Empty;
    public int Visibility { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastModifiedAt { get; set; }
}
