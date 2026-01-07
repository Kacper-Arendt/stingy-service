using Budgets.Core.Queries.Dtos;
using Budgets.Domain.ValueObjects;

namespace Budgets.Core.Queries;

public interface IBudgetQueryService
{
    Task<BudgetDto?> GetByIdAsync(BudgetId budgetId);
    Task<List<BudgetDto>> GetUserBudgetsAsync();
}
