using System.ComponentModel.DataAnnotations;
using Budgets.Domain.Enums;

namespace Budgets.Core.Commands.Dtos;

public class AddBudgetMemberDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public BudgetMemberRole Role { get; set; }
}
