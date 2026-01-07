using System.ComponentModel.DataAnnotations;

namespace Budgets.Core.Commands.Dtos;

public class CreateBudgetDto
{
    [Required]
    [StringLength(150, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }
}
