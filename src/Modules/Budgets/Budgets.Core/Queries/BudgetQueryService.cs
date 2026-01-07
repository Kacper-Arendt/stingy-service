using Budgets.Core.Queries.Dtos;
using Budgets.Core.Repositories;
using Budgets.Domain.ValueObjects;
using Shared.Abstractions.ValueObjects;
using Shared.Infrastructure.Helpers;

namespace Budgets.Core.Queries;

public class BudgetQueryService : IBudgetQueryService
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly HttpContextHelper _httpContextHelper;

    public BudgetQueryService(
        IBudgetRepository budgetRepository,
        HttpContextHelper httpContextHelper)
    {
        _budgetRepository = budgetRepository;
        _httpContextHelper = httpContextHelper;
    }

    public async Task<BudgetDto?> GetByIdAsync(BudgetId budgetId)
    {
        var user = _httpContextHelper.GetCurrentUser();
        var budget = await _budgetRepository.GetByIdAsync(budgetId);

        if (budget == null) return null;

        if (!budget.IsMember(user.Id))
            throw new UnauthorizedAccessException("You are not a member of this budget.");

        return BudgetDto.FromDomain(budget);
    }

    public async Task<List<BudgetDto>> GetUserBudgetsAsync()
    {
        var user = _httpContextHelper.GetCurrentUser();
        var budgets = await _budgetRepository.GetByUserIdAsync(user.Id);

        return budgets.Select(BudgetDto.FromDomain).ToList();
    }
}
