using System.ComponentModel.DataAnnotations;

namespace Teams.Core.Commands.Dtos;

public class AddTeamParticipantDto
{
    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    public int Role { get; set; } = 2; // Member by default
} 