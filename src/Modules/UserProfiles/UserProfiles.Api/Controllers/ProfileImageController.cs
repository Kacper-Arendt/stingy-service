using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Infrastructure.Helpers;
using UserProfiles.Core.Commands;
using UserProfiles.Core.Services;

namespace UserProfiles.Api.Controllers;

[Route("api/profiles")]
[ApiController]
[Authorize]
public class ProfileImageController : ControllerBase
{
    private readonly IProfileImageService _profileImageService;
    private readonly IUserProfileCommandService _commandService;
    private readonly HttpContextHelper _httpContextHelper;

    public ProfileImageController(
        IProfileImageService profileImageService,
        IUserProfileCommandService commandService,
        HttpContextHelper httpContextHelper)
    {
        _profileImageService = profileImageService;
        _commandService = commandService;
        _httpContextHelper = httpContextHelper;
    }

    [HttpPost("me/image")]
    public async Task<IActionResult> UploadProfileImage(IFormFile image)
    {
        if (image == null)
            return BadRequest("No image file provided.");

        if (!await _profileImageService.ValidateImageAsync(image.Length, image.ContentType))
            return BadRequest("Invalid image file. Please upload JPEG, PNG, GIF, or WebP files under 5MB.");

        var currentUser = _httpContextHelper.GetCurrentUser();
        
        try
        {
            await using var stream = image.OpenReadStream();
            var imageUrl = await _profileImageService.UploadProfileImageAsync(
                stream, 
                currentUser.Id, 
                image.FileName, 
                image.ContentType);
            
            return Ok(new { ImageUrl = imageUrl });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            return StatusCode(500, "Failed to upload image. Please try again.");
        }
    }

    [HttpDelete("me/image")]
    public async Task<IActionResult> DeleteProfileImage([FromBody] string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            return BadRequest("Image URL is required.");

        try
        {
            await _profileImageService.DeleteProfileImageAsync(imageUrl);
            return NoContent();
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            return StatusCode(500, "Failed to delete image. Please try again.");
        }
    }
}
