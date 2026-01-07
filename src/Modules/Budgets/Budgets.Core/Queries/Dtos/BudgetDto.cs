using Budgets.Domain.Entities;

namespace Budgets.Core.Queries.Dtos;

public class BudgetDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsArchived { get; set; }
    public List<BudgetMemberDto> Members { get; set; } = new();

    public static BudgetDto FromDomain(Budget budget)
    {
        return new BudgetDto
        {
            Id = budget.Id.Value,
            Name = budget.Name.Value,
            Description = budget.Description.Value,
            CreatedAt = budget.CreatedAt.Value,
            IsArchived = budget.IsArchived,
            Members = budget.Members.Select(BudgetMemberDto.FromDomain).ToList()
        };
    }
}
