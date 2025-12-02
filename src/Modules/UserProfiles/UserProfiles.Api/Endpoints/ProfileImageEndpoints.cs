using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Shared.Infrastructure.Helpers;
using UserProfiles.Core.Commands;
using UserProfiles.Core.Services;

namespace UserProfiles.Api.Endpoints;

public static class ProfileImageEndpoints
{
    public static RouteGroupBuilder MapProfileImageEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("me/image", async (IFormFile image, IProfileImageService profileImageService, IUserProfileCommandService commandService, HttpContextHelper httpContextHelper, HttpContext httpContext) =>
        {
            if (image == null)
                return Results.BadRequest("No image file provided.");

            if (!await profileImageService.ValidateImageAsync(image.Length, image.ContentType))
                return Results.BadRequest("Invalid image file. Please upload JPEG, PNG, GIF, or WebP files under 5MB.");

            var currentUser = httpContextHelper.GetCurrentUser();

            try
            {
                await using var stream = image.OpenReadStream();
                var imageUrl = await profileImageService.UploadProfileImageAsync(
                    stream,
                    currentUser.Id,
                    image.FileName,
                    image.ContentType);

                return Results.Ok(new { ImageUrl = imageUrl });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return Results.StatusCode(500);
            }
        })
        .RequireAuthorization();

        group.MapDelete("me/image", async ([FromBody] string imageUrl, IProfileImageService profileImageService) =>
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return Results.BadRequest("Image URL is required.");

            try
            {
                await profileImageService.DeleteProfileImageAsync(imageUrl);
                return Results.NoContent();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return Results.StatusCode(500);
            }
        })
        .RequireAuthorization();

        return group;
    }
}
