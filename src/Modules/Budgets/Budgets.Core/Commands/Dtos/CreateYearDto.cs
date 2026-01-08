using System.ComponentModel.DataAnnotations;

namespace Budgets.Core.Commands.Dtos;

public class CreateYearDto
{
    [Required]
    public int Value { get; set; }

    [Required]
    public Guid BudgetId { get; set; }
}
