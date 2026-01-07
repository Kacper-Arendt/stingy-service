using Budgets.Core.Commands.Dtos;
using Budgets.Core.Queries.Dtos;
using Budgets.Core.Repositories;
using Budgets.Domain.Entities;
using Budgets.Domain.Enums;
using Budgets.Domain.ValueObjects;
using Shared.Abstractions.Exceptions;
using Shared.Abstractions.Services;
using Shared.Abstractions.ValueObjects;
using Shared.Infrastructure.Helpers;

namespace Budgets.Core.Commands;

public class BudgetCommandService : IBudgetCommandService
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly HttpContextHelper _httpContextHelper;
    private readonly IClock _clock;

    public BudgetCommandService(
        IBudgetRepository budgetRepository,
        HttpContextHelper httpContextHelper,
        IClock clock)
    {
        _budgetRepository = budgetRepository;
        _httpContextHelper = httpContextHelper;
        _clock = clock;
    }

    public async Task<BudgetDto> CreateAsync(CreateBudgetDto dto)
    {
        var user = _httpContextHelper.GetCurrentUser();
        var budgetId = new BudgetId(Guid.NewGuid());

        var budget = new Budget.BudgetBuilder()
            .WithId(budgetId)
            .WithName(new BudgetName(dto.Name))
            .WithDescription(new BudgetDescription(dto.Description ?? string.Empty))
            .WithCreatedAt(new CreatedAtDate(_clock.UtcNow))
            .Build();

        // Add creator as Owner
        var ownerMember = new BudgetMember.BudgetMemberBuilder()
            .WithBudgetId(budgetId)
            .WithUserId(user.Id)
            .WithEmail(user.Email)
            .WithRole(BudgetMemberRole.Owner)
            .WithStatus(BudgetMemberStatus.Active)
            .WithJoinedAt(new CreatedAtDate(_clock.UtcNow))
            .Build();

        budget.AddMember(ownerMember);
        await _budgetRepository.SaveAsync(budget);

        return BudgetDto.FromDomain(budget);
    }

    public async Task AddMemberAsync(BudgetId budgetId, AddBudgetMemberDto dto)
    {
        var currentUser = _httpContextHelper.GetCurrentUser();
        var budget = await _budgetRepository.GetByIdAsync(budgetId);

        if (budget == null)
            throw new InvalidOperationException("Budget not found.");

        if (!budget.IsAdmin(currentUser.Id))
            throw new UnauthorizedAccessException("Only budget owners and admins can add members.");

        // Check if user is already a member
        if (budget.Members.Any(m => m.Email.Value.Equals(dto.Email, StringComparison.OrdinalIgnoreCase)))
            throw new DomainModelArgumentException($"User with email '{dto.Email}' is already a member of this budget.");

        var newMember = new BudgetMember.BudgetMemberBuilder()
            .WithBudgetId(budgetId)
            .WithUserId(new UserId(Guid.NewGuid())) // Will be updated when user accepts invitation
            .WithEmail(new Email(dto.Email))
            .WithRole(dto.Role)
            .WithStatus(BudgetMemberStatus.Invited)
            .WithJoinedAt(new CreatedAtDate(_clock.UtcNow))
            .Build();

        await _budgetRepository.AddMemberAsync(budgetId, newMember);
    }
}
