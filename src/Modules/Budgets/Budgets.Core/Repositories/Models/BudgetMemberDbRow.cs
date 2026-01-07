namespace Budgets.Core.Repositories.Models;

public class BudgetMemberDbRow
{
    public Guid BudgetId { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public int Role { get; set; }
    public int Status { get; set; }
    public DateTime JoinedAt { get; set; }
}
