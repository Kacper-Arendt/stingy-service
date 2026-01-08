using Budgets.Domain.Entities;
using Budgets.Domain.ValueObjects;

namespace Budgets.Core.Repositories;

public interface IYearRepository
{
    Task<BudgetYear?> GetByIdAsync(YearId yearId);
    Task<BudgetYear?> GetByBudgetIdAndValueAsync(BudgetId budgetId, YearValue value);
    Task<List<BudgetYear>> GetByBudgetIdAsync(BudgetId budgetId);
    Task SaveAsync(BudgetYear year);
    Task UpdateAsync(BudgetYear year);
    Task DeleteAsync(YearId yearId);
}
