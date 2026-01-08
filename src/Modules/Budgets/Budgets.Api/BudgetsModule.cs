using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Budgets.Api.Endpoints;
using Budgets.Core;
using Budgets.Core.Authorization;

namespace Budgets.Api;

public static class BudgetsModule
{
    public static WebApplicationBuilder RegisterBudgetsModule(this WebApplicationBuilder builder)
    {
        builder.Services.AddBudgetsCore();

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.HasAccessToBudget, policy =>
                policy.Requirements.Add(new HasAccessToBudget()));
        });

        return builder;
    }

    public static WebApplication UseBudgetsModule(this WebApplication app)
    {
        app.MapGroup("api/budgets")
            .RequireAuthorization()
            .MapBudgetsEndpoints()
            .MapBudgetYearsEndpoints()
            .WithTags("Budgets");
        
        return app;
    }
}
