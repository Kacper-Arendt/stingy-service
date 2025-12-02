using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Infrastructure.Helpers;
using UserProfiles.Core.Commands;
using UserProfiles.Core.Commands.Dtos;
using UserProfiles.Core.Queries;

namespace UserProfiles.Api.Controllers;

[Route("api/profiles/me/tours")]
[ApiController]
[Authorize]
public class ProductTourController : ControllerBase
{
    private readonly IProductTourCommandService _commandService;
    private readonly IProductTourQueryService _queryService;
    private readonly HttpContextHelper _httpContextHelper;

    public ProductTourController(
        IProductTourCommandService commandService,
        IProductTourQueryService queryService,
        HttpContextHelper httpContextHelper)
    {
        _commandService = commandService;
        _queryService = queryService;
        _httpContextHelper = httpContextHelper;
    }

    [HttpPost]
    public async Task<IActionResult> SetTourStatus([FromBody] SetTourStatusDto dto)
    {
        var currentUser = _httpContextHelper.GetCurrentUser();
        
        await _commandService.SetTourStatusAsync(currentUser.Id, dto);
        
        return Ok(new { Message = "Tour status updated successfully" });
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTourStatuses()
    {
        var currentUser = _httpContextHelper.GetCurrentUser();
        
        var result = await _queryService.GetAllTourStatusesAsync(currentUser.Id);
        
        return Ok(result);
    }
}

