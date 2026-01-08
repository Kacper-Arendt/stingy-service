using Budgets.Domain.Entities;

namespace Budgets.Core.Queries.Dtos;

public class YearDto
{
    public Guid Id { get; set; }
    public int Value { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid BudgetId { get; set; }

    public static YearDto FromDomain(BudgetYear year)
    {
        return new YearDto
        {
            Id = year.Id.Value,
            Value = year.Value.Value,
            Status = year.Status.ToString(),
            BudgetId = year.BudgetId.Value
        };
    }
}
