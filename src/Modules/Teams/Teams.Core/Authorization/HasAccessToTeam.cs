using Microsoft.AspNetCore.Authorization;

namespace Teams.Core.Authorization;

public class HasAccessToTeam : IAuthorizationRequirement
{
    public string TeamIdParameterName { get; }

    public HasAccessToTeam(string teamIdParameterName = "teamId")
    {
        TeamIdParameterName = teamIdParameterName;
    }
} 