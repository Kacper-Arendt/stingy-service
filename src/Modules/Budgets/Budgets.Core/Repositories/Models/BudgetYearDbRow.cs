namespace Budgets.Core.Repositories.Models;

public class BudgetYearDbRow
{
    public Guid Id { get; set; }
    public int Value { get; set; }
    public int Status { get; set; }
    public Guid BudgetId { get; set; }
}
