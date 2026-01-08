using Budgets.Core.Commands.Dtos;
using Budgets.Core.Queries.Dtos;
using Budgets.Core.Repositories;
using Budgets.Domain.Entities;
using Budgets.Domain.Enums;
using Budgets.Domain.ValueObjects;
using Shared.Abstractions.Exceptions;
using Shared.Abstractions.Services;
using Shared.Infrastructure.Helpers;

namespace Budgets.Core.Commands;

public class YearCommandService : IYearCommandService
{
    private readonly IYearRepository _yearRepository;
    private readonly IBudgetRepository _budgetRepository;
    private readonly HttpContextHelper _httpContextHelper;

    public YearCommandService(
        IYearRepository yearRepository,
        IBudgetRepository budgetRepository,
        HttpContextHelper httpContextHelper)
    {
        _yearRepository = yearRepository;
        _budgetRepository = budgetRepository;
        _httpContextHelper = httpContextHelper;
    }

    public async Task<YearDto> CreateAsync(CreateYearDto dto)
    {
        var user = _httpContextHelper.GetCurrentUser();
        var budgetId = new BudgetId(dto.BudgetId);

        // Check if budget exists and user has access
        var budget = await _budgetRepository.GetByIdAsync(budgetId);
        if (budget == null)
            throw new InvalidOperationException("Budget not found.");

        if (!budget.IsMember(user.Id))
            throw new UnauthorizedAccessException("You are not a member of this budget.");

        // Check if year with this value already exists
        var yearValue = new YearValue(dto.Value);
        var existingYear = await _yearRepository.GetByBudgetIdAndValueAsync(budgetId, yearValue);
        if (existingYear != null)
            throw new DomainModelArgumentException($"Year {dto.Value} already exists for this budget.");

        var yearId = new YearId(Guid.NewGuid());
        var year = new BudgetYear.BudgetYearBuilder()
            .WithId(yearId)
            .WithBudgetId(budgetId)
            .WithValue(yearValue)
            .WithStatus(YearStatus.Active)
            .Build();

        await _yearRepository.SaveAsync(year);

        return YearDto.FromDomain(year);
    }

    public async Task<YearDto> UpdateAsync(YearId yearId, UpdateYearDto dto)
    {
        var user = _httpContextHelper.GetCurrentUser();
        var year = await _yearRepository.GetByIdAsync(yearId);

        if (year == null)
            throw new InvalidOperationException("Year not found.");

        // Check if budget exists and user has access
        var budget = await _budgetRepository.GetByIdAsync(year.BudgetId);
        if (budget == null)
            throw new InvalidOperationException("Budget not found.");

        if (!budget.IsMember(user.Id))
            throw new UnauthorizedAccessException("You are not a member of this budget.");

        // Update value if provided
        if (dto.Value.HasValue)
        {
            var newYearValue = new YearValue(dto.Value.Value);
            
            // Check if another year with this value already exists
            var existingYear = await _yearRepository.GetByBudgetIdAndValueAsync(year.BudgetId, newYearValue);
            if (existingYear != null && existingYear.Id.Value != yearId.Value)
                throw new DomainModelArgumentException($"Year {dto.Value.Value} already exists for this budget.");

            // Create updated year with new value
            year = new BudgetYear.BudgetYearBuilder()
                .WithId(year.Id)
                .WithBudgetId(year.BudgetId)
                .WithValue(newYearValue)
                .WithStatus(dto.Status ?? year.Status)
                .Build();
        }
        await _yearRepository.UpdateAsync(year);

        return YearDto.FromDomain(year);
    }

    public async Task ArchiveAsync(YearId yearId)
    {
        var user = _httpContextHelper.GetCurrentUser();
        var year = await _yearRepository.GetByIdAsync(yearId);

        if (year == null)
            throw new InvalidOperationException("Year not found.");

        // Check if budget exists and user has access
        var budget = await _budgetRepository.GetByIdAsync(year.BudgetId);
        if (budget == null)
            throw new InvalidOperationException("Budget not found.");

        if (!budget.IsMember(user.Id))
            throw new UnauthorizedAccessException("You are not a member of this budget.");

        year.Archive();
        await _yearRepository.UpdateAsync(year);
    }

    public async Task DeleteAsync(YearId yearId)
    {
        var user = _httpContextHelper.GetCurrentUser();
        var year = await _yearRepository.GetByIdAsync(yearId);

        if (year == null)
            throw new InvalidOperationException("Year not found.");

        // Check if budget exists and user has access
        var budget = await _budgetRepository.GetByIdAsync(year.BudgetId);
        if (budget == null)
            throw new InvalidOperationException("Budget not found.");

        if (!budget.IsMember(user.Id))
            throw new UnauthorizedAccessException("You are not a member of this budget.");

        await _yearRepository.DeleteAsync(yearId);
    }
}
