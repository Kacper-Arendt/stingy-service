using System.ComponentModel.DataAnnotations;

namespace Teams.Core.Commands.Dtos;

public class CreateRetroFromTeamDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Title { get; set; } = string.Empty;
} 