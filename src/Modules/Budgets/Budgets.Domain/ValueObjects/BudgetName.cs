using Shared.Abstractions.Exceptions;
using Shared.Abstractions.ValueObjects;

namespace Budgets.Domain.ValueObjects;

public class BudgetName : ValueObject
{
    public string Value { get; }

    public BudgetName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ValueObjectInvalidTypeException("BudgetName cannot be empty.");

        if (value.Length < 1)
            throw new ValueObjectInvalidTypeException("BudgetName must be at least 1 character long.");

        if (value.Length > 150)
            throw new ValueObjectInvalidTypeException("BudgetName cannot exceed 150 characters.");

        Value = value;
    }

    public static implicit operator string(BudgetName budgetName) => budgetName.Value;
    public static explicit operator BudgetName(string value) => new(value);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
