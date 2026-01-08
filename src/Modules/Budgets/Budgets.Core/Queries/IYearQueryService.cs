using Budgets.Core.Queries.Dtos;
using Budgets.Domain.ValueObjects;

namespace Budgets.Core.Queries;

public interface IYearQueryService
{
    Task<List<YearDto>> GetByBudgetIdAsync(BudgetId budgetId);
    Task<YearDto?> GetByBudgetIdAndValueAsync(BudgetId budgetId, int value);
    Task<YearDto?> GetByIdAsync(YearId yearId);
}
