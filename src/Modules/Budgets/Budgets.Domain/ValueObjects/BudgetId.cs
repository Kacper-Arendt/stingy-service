using Shared.Abstractions.Exceptions;
using Shared.Abstractions.ValueObjects;

namespace Budgets.Domain.ValueObjects;

public class BudgetId : ValueObject
{
    public Guid Value { get; }

    public BudgetId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ValueObjectInvalidTypeException("BudgetId cannot be empty.");

        Value = value;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(BudgetId budgetId) => budgetId.Value;
    public static explicit operator BudgetId(Guid value) => new(value);
}
