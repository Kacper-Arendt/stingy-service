using Microsoft.AspNetCore.Authorization;

namespace Budgets.Core.Authorization;

public class HasAccessToBudget : IAuthorizationRequirement
{
    public string BudgetIdParameterName { get; }

    public HasAccessToBudget(string budgetIdParameterName = "budgetId")
    {
        BudgetIdParameterName = budgetIdParameterName;
    }
}
