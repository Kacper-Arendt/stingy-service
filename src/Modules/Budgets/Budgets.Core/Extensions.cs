using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Budgets.Core.Commands;
using Budgets.Core.Queries;
using Budgets.Core.Repositories;
using Budgets.Core.Authorization;

namespace Budgets.Core;

public static class Extensions
{
    public static IServiceCollection AddBudgetsCore(this IServiceCollection services)
    {
        // Command Services
        services.AddScoped<IBudgetCommandService, BudgetCommandService>();
        services.AddScoped<IYearCommandService, YearCommandService>();

        // Query Services
        services.AddScoped<IBudgetQueryService, BudgetQueryService>();
        services.AddScoped<IYearQueryService, YearQueryService>();

        // Repositories
        services.AddScoped<IBudgetRepository, BudgetRepository>();
        services.AddScoped<IYearRepository, YearRepository>();

        // Authorization Handlers
        services.AddScoped<IAuthorizationHandler, HasAccessToBudgetHandler>();

        return services;
    }
}
