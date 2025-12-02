using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Infrastructure.Helpers;
using UserProfiles.Core.Commands;
using UserProfiles.Core.Commands.Dtos;
using UserProfiles.Core.Queries;

namespace UserProfiles.Api.Controllers;

[Route("api/profiles")]
[ApiController]
[Authorize]
public class UserProfilesController : ControllerBase
{
    private readonly IUserProfileCommandService _commandService;
    private readonly IUserProfileQueryService _queryService;
    private readonly HttpContextHelper _httpContextHelper;

    public UserProfilesController(
        IUserProfileCommandService commandService,
        IUserProfileQueryService queryService,
        HttpContextHelper httpContextHelper)
    {
        _commandService = commandService;
        _queryService = queryService;
        _httpContextHelper = httpContextHelper;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var currentUser = _httpContextHelper.GetCurrentUser();
        var result = await _queryService.GetByUserIdAsync(currentUser.Id);
        
        if (result == null) 
            return NotFound("User profile not found.");
            
        return Ok(result);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateUserProfileDto dto)
    {
        var currentUser = _httpContextHelper.GetCurrentUser();
        var result = await _commandService.UpdateAsync(currentUser.Id, dto);
        return Ok(result);
    }

    [HttpPost("me")]
    public async Task<IActionResult> CreateMyProfile([FromBody] CreateUserProfileDto dto)
    {
        var currentUser = _httpContextHelper.GetCurrentUser();
        var result = await _commandService.CreateAsync(currentUser.Id, dto);
        return Ok(result);
    }




}
