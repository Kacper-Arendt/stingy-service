using Budgets.Core.Queries.Dtos;
using Budgets.Core.Repositories;
using Budgets.Domain.ValueObjects;
using Shared.Infrastructure.Helpers;

namespace Budgets.Core.Queries;

public class YearQueryService : IYearQueryService
{
    private readonly IYearRepository _yearRepository;
    private readonly IBudgetRepository _budgetRepository;
    private readonly HttpContextHelper _httpContextHelper;

    public YearQueryService(
        IYearRepository yearRepository,
        IBudgetRepository budgetRepository,
        HttpContextHelper httpContextHelper)
    {
        _yearRepository = yearRepository;
        _budgetRepository = budgetRepository;
        _httpContextHelper = httpContextHelper;
    }

    public async Task<List<YearDto>> GetByBudgetIdAsync(BudgetId budgetId)
    {
        var user = _httpContextHelper.GetCurrentUser();

        // Check if budget exists and user has access
        var budget = await _budgetRepository.GetByIdAsync(budgetId);
        if (budget == null)
            throw new InvalidOperationException("Budget not found.");

        if (!budget.IsMember(user.Id))
            throw new UnauthorizedAccessException("You are not a member of this budget.");

        var years = await _yearRepository.GetByBudgetIdAsync(budgetId);
        return years.Select(YearDto.FromDomain).ToList();
    }

    public async Task<YearDto?> GetByBudgetIdAndValueAsync(BudgetId budgetId, int value)
    {
        var user = _httpContextHelper.GetCurrentUser();

        // Check if budget exists and user has access
        var budget = await _budgetRepository.GetByIdAsync(budgetId);
        if (budget == null)
            throw new InvalidOperationException("Budget not found.");

        if (!budget.IsMember(user.Id))
            throw new UnauthorizedAccessException("You are not a member of this budget.");

        var yearValue = new YearValue(value);
        var year = await _yearRepository.GetByBudgetIdAndValueAsync(budgetId, yearValue);
        
        return year == null ? null : YearDto.FromDomain(year);
    }

    public async Task<YearDto?> GetByIdAsync(YearId yearId)
    {
        var user = _httpContextHelper.GetCurrentUser();
        var year = await _yearRepository.GetByIdAsync(yearId);

        if (year == null) return null;

        // Check if budget exists and user has access
        var budget = await _budgetRepository.GetByIdAsync(year.BudgetId);
        if (budget == null)
            throw new InvalidOperationException("Budget not found.");

        if (!budget.IsMember(user.Id))
            throw new UnauthorizedAccessException("You are not a member of this budget.");

        return YearDto.FromDomain(year);
    }
}
