using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Budgets.Core.Repositories;
using Budgets.Domain.ValueObjects;
using Shared.Abstractions.ValueObjects;
using Shared.Infrastructure.Helpers;

namespace Budgets.Core.Authorization;

public class HasAccessToBudgetHandler : AuthorizationHandler<HasAccessToBudget>
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly HttpContextHelper _httpContextHelper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HasAccessToBudgetHandler(
        IBudgetRepository budgetRepository,
        HttpContextHelper httpContextHelper,
        IHttpContextAccessor httpContextAccessor)
    {
        _budgetRepository = budgetRepository;
        _httpContextHelper = httpContextHelper;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        HasAccessToBudget requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            context.Fail();
            return;
        }

        var budgetIdValue = httpContext.Request.RouteValues[requirement.BudgetIdParameterName]?.ToString();
        if (!Guid.TryParse(budgetIdValue, out var budgetIdGuid))
        {
            context.Fail();
            return;
        }

        var user = _httpContextHelper.GetCurrentUser();
        var budgetId = new BudgetId(budgetIdGuid);

        var isMember = await _budgetRepository.IsMemberAsync(user.Id, budgetId);
        if (isMember)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }
}
