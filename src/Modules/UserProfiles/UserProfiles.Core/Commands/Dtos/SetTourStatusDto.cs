using System.ComponentModel.DataAnnotations;
using UserProfiles.Domain.Enums;

namespace UserProfiles.Core.Commands.Dtos;

public class SetTourStatusDto
{
    [Required]
    [StringLength(100)]
    public string TourKey { get; set; } = string.Empty;

    [Required]
    public TourStatus Status { get; set; }
}

