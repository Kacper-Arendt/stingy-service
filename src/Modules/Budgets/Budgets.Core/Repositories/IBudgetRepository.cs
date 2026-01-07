using Budgets.Domain.Entities;
using Budgets.Domain.Enums;
using Budgets.Domain.ValueObjects;
using Shared.Abstractions.ValueObjects;

namespace Budgets.Core.Repositories;

public interface IBudgetRepository
{
    Task<Budget?> GetByIdAsync(BudgetId budgetId);
    Task<List<Budget>> GetByUserIdAsync(UserId userId, BudgetFilter filter = BudgetFilter.Active);
    Task SaveAsync(Budget budget);
    Task AddMemberAsync(BudgetId budgetId, BudgetMember member);
    Task RemoveMemberAsync(BudgetId budgetId, UserId userId);
    Task<bool> IsMemberAsync(UserId userId, BudgetId budgetId);
}
