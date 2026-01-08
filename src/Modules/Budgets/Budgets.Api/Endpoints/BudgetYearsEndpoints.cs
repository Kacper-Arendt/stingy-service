using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Budgets.Core.Authorization;
using Budgets.Core.Commands;
using Budgets.Core.Commands.Dtos;
using Budgets.Core.Queries;
using Budgets.Domain.ValueObjects;

namespace Budgets.Api.Endpoints;

public static class BudgetYearsEndpoints
{
    public static RouteGroupBuilder MapBudgetYearsEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("{budgetId:guid}/years", async (
            Guid budgetId,
            CreateYearDto dto,
            IYearCommandService commandService) =>
        {
            dto.BudgetId = budgetId;
            var result = await commandService.CreateAsync(dto);
            return TypedResults.Ok(result);
        }).RequireAuthorization(Policies.HasAccessToBudget);

        group.MapGet("{budgetId:guid}/years", async (
            Guid budgetId,
            IYearQueryService queryService) =>
        {
            var result = await queryService.GetByBudgetIdAsync(new BudgetId(budgetId));
            return TypedResults.Ok(result);
        }).RequireAuthorization(Policies.HasAccessToBudget);

        group.MapGet("{budgetId:guid}/years/{value:int}", async (
            Guid budgetId,
            int value,
            IYearQueryService queryService) =>
        {
            var result = await queryService.GetByBudgetIdAndValueAsync(new BudgetId(budgetId), value);
            return result is null ? Results.NotFound() : TypedResults.Ok(result);
        }).RequireAuthorization(Policies.HasAccessToBudget);

        group.MapPut("{budgetId:guid}/years/{yearId:guid}", async (
            Guid budgetId,
            Guid yearId,
            UpdateYearDto dto,
            IYearCommandService commandService) =>
        {
            var result = await commandService.UpdateAsync(new YearId(yearId), dto);
            return TypedResults.Ok(result);
        }).RequireAuthorization(Policies.HasAccessToBudget);

        group.MapPost("{budgetId:guid}/years/{yearId:guid}/archive", async (
            Guid budgetId,
            Guid yearId,
            IYearCommandService commandService) =>
        {
            await commandService.ArchiveAsync(new YearId(yearId));
            return TypedResults.Ok();
        }).RequireAuthorization(Policies.HasAccessToBudget);

        group.MapDelete("{budgetId:guid}/years/{yearId:guid}", async (
            Guid budgetId,
            Guid yearId,
            IYearCommandService commandService) =>
        {
            await commandService.DeleteAsync(new YearId(yearId));
            return TypedResults.Ok();
        }).RequireAuthorization(Policies.HasAccessToBudget);

        return group;
    }
}
