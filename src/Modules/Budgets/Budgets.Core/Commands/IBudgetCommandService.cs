using Budgets.Core.Commands.Dtos;
using Budgets.Core.Queries.Dtos;
using Budgets.Domain.ValueObjects;

namespace Budgets.Core.Commands;

public interface IBudgetCommandService
{
    Task<BudgetDto> CreateAsync(CreateBudgetDto dto);
    Task AddMemberAsync(BudgetId budgetId, AddBudgetMemberDto dto);
}
