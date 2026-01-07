using Budgets.Domain.Enums;
using Budgets.Domain.ValueObjects;
using Shared.Abstractions.Exceptions;
using Shared.Abstractions.ValueObjects;

namespace Budgets.Domain.Entities;

public class BudgetMember
{
    public UserId UserId { get; private set; } = null!;
    public BudgetId BudgetId { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public BudgetMemberRole Role { get; private set; }
    public BudgetMemberStatus Status { get; private set; }
    public CreatedAtDate JoinedAt { get; private set; } = null!;

    private BudgetMember()
    {
        // EF Core constructor
    }

    public void ChangeRole(BudgetMemberRole newRole)
    {
        if (Role == newRole)
            throw new InvalidOperationException("New role must be different from current role.");

        Role = newRole;
    }

    public void Activate()
    {
        if (Status == BudgetMemberStatus.Active)
            throw new InvalidOperationException("Member is already active.");

        Status = BudgetMemberStatus.Active;
    }

    #region Builder

    public class BudgetMemberBuilder
    {
        private UserId _userId = null!;
        private BudgetId _budgetId = null!;
        private Email _email = null!;
        private BudgetMemberRole _role;
        private BudgetMemberStatus _status;
        private CreatedAtDate _joinedAt = null!;

        public BudgetMemberBuilder WithUserId(UserId userId)
        {
            _userId = userId;
            return this;
        }

        public BudgetMemberBuilder WithBudgetId(BudgetId budgetId)
        {
            _budgetId = budgetId;
            return this;
        }

        public BudgetMemberBuilder WithEmail(Email email)
        {
            _email = email;
            return this;
        }

        public BudgetMemberBuilder WithRole(BudgetMemberRole role)
        {
            _role = role;
            return this;
        }

        public BudgetMemberBuilder WithStatus(BudgetMemberStatus status)
        {
            _status = status;
            return this;
        }

        public BudgetMemberBuilder WithJoinedAt(CreatedAtDate joinedAt)
        {
            _joinedAt = joinedAt;
            return this;
        }

        public BudgetMember Build()
        {
            if (_userId == null) throw new DomainModelArgumentException("UserId is required.");
            if (_budgetId == null) throw new DomainModelArgumentException("BudgetId is required.");
            if (_email == null) throw new DomainModelArgumentException("Email is required.");
            if (_joinedAt == null) throw new DomainModelArgumentException("JoinedAt is required.");

            return new BudgetMember
            {
                UserId = _userId,
                BudgetId = _budgetId,
                Email = _email,
                Role = _role,
                Status = _status,
                JoinedAt = _joinedAt
            };
        }
    }

    #endregion
}
