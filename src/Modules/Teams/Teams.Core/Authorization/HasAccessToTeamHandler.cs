using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Teams.Core.Repositories;
using Teams.Domain.ValueObjects;
using Shared.Abstractions.ValueObjects;
using Shared.Infrastructure.Helpers;

namespace Teams.Core.Authorization;

public class HasAccessToTeamHandler : AuthorizationHandler<HasAccessToTeam>
{
    private readonly ITeamRepository _teamRepository;
    private readonly HttpContextHelper _httpContextHelper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HasAccessToTeamHandler(
        ITeamRepository teamRepository,
        HttpContextHelper httpContextHelper,
        IHttpContextAccessor httpContextAccessor)
    {
        _teamRepository = teamRepository;
        _httpContextHelper = httpContextHelper;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        HasAccessToTeam requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            context.Fail();
            return;
        }

        var teamIdValue = httpContext.Request.RouteValues[requirement.TeamIdParameterName]?.ToString();
        if (!Guid.TryParse(teamIdValue, out var teamIdGuid))
        {
            context.Fail();
            return;
        }

        var user = _httpContextHelper.GetCurrentUser();
        var teamId = new TeamId(teamIdGuid);

        var isParticipant = await _teamRepository.IsParticipantAsync(user.Id, teamId);
        if (isParticipant)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }
} 