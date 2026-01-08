using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Budgets.Core.Authorization;
using Budgets.Core.Commands;
using Budgets.Core.Commands.Dtos;
using Budgets.Core.Queries;
using Budgets.Domain.Enums;
using Budgets.Domain.ValueObjects;

namespace Budgets.Api.Endpoints;

public static class BudgetsEndpoints
{
    public static RouteGroupBuilder MapBudgetsEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("", async (
            IBudgetCommandService commandService,
            CreateBudgetDto dto) =>
        {
            var result = await commandService.CreateAsync(dto);
            return TypedResults.Ok(result);
        });

        group.MapGet("", async (
            IBudgetQueryService queryService,
            BudgetFilter status = BudgetFilter.All) =>
        {
            var result = await queryService.GetUserBudgetsAsync(status);
            return TypedResults.Ok(result);
        });

        group.MapGet("{budgetId:guid}", async (
            Guid budgetId,
            IBudgetQueryService queryService) =>
        {
            var result = await queryService.GetByIdAsync(new BudgetId(budgetId));
            return result is null ? Results.NotFound() : TypedResults.Ok(result);
        }).RequireAuthorization(Policies.HasAccessToBudget);

        group.MapPost("{budgetId:guid}/members", async (
            Guid budgetId,
            AddBudgetMemberDto dto,
            IBudgetCommandService commandService) =>
        {
            await commandService.AddMemberAsync(new BudgetId(budgetId), dto);
            return TypedResults.Ok();
        }).RequireAuthorization(Policies.HasAccessToBudget);

        return group;
    }
}
