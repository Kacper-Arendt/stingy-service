using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Abstractions.ValueObjects;
using Shared.Infrastructure.Helpers;
using UserProfiles.Core.Queries;

namespace UserProfiles.Api.Controllers;

[Route("api/profiles")]
[ApiController]
[Authorize]
public class PublicProfilesController : ControllerBase
{
    private readonly IUserProfileQueryService _queryService;
    private readonly HttpContextHelper _httpContextHelper;

    public PublicProfilesController(
        IUserProfileQueryService queryService,
        HttpContextHelper httpContextHelper)
    {
        _queryService = queryService;
        _httpContextHelper = httpContextHelper;
    }

    [HttpGet("{userId}/public")]
    public async Task<IActionResult> GetPublicProfile([FromRoute] Guid userId)
    {
        var currentUser = _httpContextHelper.GetCurrentUser();
        var result = await _queryService.GetPublicProfileAsync(new UserId(userId), currentUser.Id);
        
        if (result == null) 
            return NotFound("Profile not found or not accessible.");
            
        return Ok(result);
    }


}
