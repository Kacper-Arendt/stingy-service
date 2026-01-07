using Budgets.Domain.Enums;
using Budgets.Domain.Entities;

namespace Budgets.Core.Queries.Dtos;

public class BudgetMemberDto
{
    public Guid UserId { get; set; }
    public Guid BudgetId { get; set; }
    public string Email { get; set; } = string.Empty;
    public BudgetMemberRole Role { get; set; }
    public BudgetMemberStatus Status { get; set; }
    public DateTime JoinedAt { get; set; }

    public static BudgetMemberDto FromDomain(BudgetMember member)
    {
        return new BudgetMemberDto
        {
            UserId = member.UserId.Value,
            BudgetId = member.BudgetId.Value,
            Email = member.Email.Value,
            Role = member.Role,
            Status = member.Status,
            JoinedAt = member.JoinedAt.Value
        };
    }
}
