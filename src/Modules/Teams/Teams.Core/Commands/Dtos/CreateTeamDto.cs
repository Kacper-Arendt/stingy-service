using System.ComponentModel.DataAnnotations;

namespace Teams.Core.Commands.Dtos;

public class CreateTeamDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }
} 