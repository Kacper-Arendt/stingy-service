using Shared.Abstractions.Exceptions;
using Shared.Abstractions.ValueObjects;

namespace Budgets.Domain.ValueObjects;

public class BudgetDescription : ValueObject
{
    public string Value { get; }

    public BudgetDescription(string value)
    {
        if (value != null && value.Length > 500)
            throw new ValueObjectInvalidTypeException("BudgetDescription cannot exceed 500 characters.");

        Value = value ?? string.Empty;
    }

    public static implicit operator string(BudgetDescription budgetDescription) => budgetDescription.Value;
    public static explicit operator BudgetDescription(string value) => new(value);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
