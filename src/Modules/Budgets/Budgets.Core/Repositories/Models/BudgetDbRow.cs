namespace Budgets.Core.Repositories.Models;

public class BudgetDbRow
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsArchived { get; set; }
}
