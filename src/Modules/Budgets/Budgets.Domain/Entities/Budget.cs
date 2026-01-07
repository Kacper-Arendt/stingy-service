using Budgets.Domain.Enums;
using Budgets.Domain.ValueObjects;
using Shared.Abstractions.Exceptions;
using Shared.Abstractions.ValueObjects;

namespace Budgets.Domain.Entities;

public class Budget
{
    public BudgetId Id { get; private set; } = null!;
    public BudgetName Name { get; private set; } = null!;
    public BudgetDescription Description { get; private set; } = null!;
    public CreatedAtDate CreatedAt { get; private set; } = null!;
    public List<BudgetMember> Members { get; private set; } = [];

    private Budget()
    {
        // EF Core constructor
    }

    public void AddMember(BudgetMember member)
    {
        if (member == null)
            throw new DomainModelArgumentException("Member cannot be null.");

        if (Members.Any(m => m.UserId.Value == member.UserId.Value))
            throw new DomainModelArgumentException($"User {member.UserId.Value} is already a member of this budget.");

        Members.Add(member);
    }

    public void RemoveMember(UserId userId)
    {
        var member = Members.FirstOrDefault(m => m.UserId.Value == userId.Value);
        if (member == null)
            throw new DomainModelArgumentException($"User {userId.Value} is not a member of this budget.");

        Members.Remove(member);
    }

    public bool IsMember(UserId userId)
    {
        return Members.Any(m => m.UserId.Value == userId.Value && m.Status == BudgetMemberStatus.Active);
    }

    public bool IsOwner(UserId userId)
    {
        return Members.Any(m => m.UserId.Value == userId.Value && m.Role == BudgetMemberRole.Owner && m.Status == BudgetMemberStatus.Active);
    }

    public bool IsAdmin(UserId userId)
    {
        return Members.Any(m => m.UserId.Value == userId.Value && 
                                m.Role is BudgetMemberRole.Owner or BudgetMemberRole.Admin && 
                                m.Status == BudgetMemberStatus.Active);
    }

    #region Builder

    public class BudgetBuilder
    {
        private BudgetId _id = null!;
        private BudgetName _name = null!;
        private BudgetDescription _description = new BudgetDescription(string.Empty);
        private CreatedAtDate _createdAt = new CreatedAtDate(DateTime.UtcNow);

        public BudgetBuilder WithId(BudgetId id)
        {
            _id = id;
            return this;
        }

        public BudgetBuilder WithName(BudgetName name)
        {
            _name = name;
            return this;
        }

        public BudgetBuilder WithDescription(BudgetDescription description)
        {
            _description = description ?? new BudgetDescription(string.Empty);
            return this;
        }

        public BudgetBuilder WithCreatedAt(CreatedAtDate createdAt)
        {
            _createdAt = createdAt;
            return this;
        }

        public Budget Build()
        {
            if (_id == null) throw new DomainModelArgumentException("BudgetId is required.");
            if (_name == null) throw new DomainModelArgumentException("BudgetName is required.");

            return new Budget
            {
                Id = _id,
                Name = _name,
                Description = _description,
                CreatedAt = _createdAt
            };
        }
    }

    #endregion
}
