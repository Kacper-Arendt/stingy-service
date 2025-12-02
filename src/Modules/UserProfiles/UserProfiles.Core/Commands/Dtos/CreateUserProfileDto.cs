using System.ComponentModel.DataAnnotations;
using UserProfiles.Domain.Enums;

namespace UserProfiles.Core.Commands.Dtos;

public class CreateUserProfileDto
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string DisplayName { get; set; } = string.Empty;

    [Url]
    [StringLength(500)]
    public string? ProfileImageUrl { get; set; }

    [StringLength(500)]
    public string? Bio { get; set; }

    [StringLength(50)]
    public string? TimeZone { get; set; } = "Europe/Warsaw";

    public ProfileVisibilityLevel Visibility { get; set; } = ProfileVisibilityLevel.Public;
}
