using Budgets.Domain.Enums;
using Budgets.Domain.ValueObjects;
using Shared.Abstractions.Exceptions;

namespace Budgets.Domain.Entities;

public class BudgetYear
{
    public YearId Id { get; private set; } = null!;
    public YearValue Value { get; private set; } = null!;
    public YearStatus Status { get; private set; }
    public BudgetId BudgetId { get; private set; } = null!;

    private BudgetYear()
    {
        // EF Core constructor
    }

    public void Archive()
    {
        if (Status == YearStatus.Archived)
            throw new InvalidOperationException("Year is already archived.");

        Status = YearStatus.Archived;
    }

    public void Activate()
    {
        if (Status == YearStatus.Active)
            throw new InvalidOperationException("Year is already active.");

        Status = YearStatus.Active;
    }

    #region Builder

    public class BudgetYearBuilder
    {
        private YearId _id = null!;
        private YearValue _value = null!;
        private YearStatus _status = YearStatus.Active;
        private BudgetId _budgetId = null!;

        public BudgetYearBuilder WithId(YearId id)
        {
            _id = id;
            return this;
        }

        public BudgetYearBuilder WithValue(YearValue value)
        {
            _value = value;
            return this;
        }

        public BudgetYearBuilder WithStatus(YearStatus status)
        {
            _status = status;
            return this;
        }

        public BudgetYearBuilder WithBudgetId(BudgetId budgetId)
        {
            _budgetId = budgetId;
            return this;
        }

        public BudgetYear Build()
        {
            if (_id == null) throw new DomainModelArgumentException("YearId is required.");
            if (_value == null) throw new DomainModelArgumentException("YearValue is required.");
            if (_budgetId == null) throw new DomainModelArgumentException("BudgetId is required.");

            return new BudgetYear
            {
                Id = _id,
                Value = _value,
                Status = _status,
                BudgetId = _budgetId
            };
        }
    }

    #endregion
}
