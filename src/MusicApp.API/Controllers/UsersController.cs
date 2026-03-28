using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicApp.Application.Common.DTOs;
using MusicApp.Application.Users.Commands.FollowUser;
using MusicApp.Application.Users.Commands.LikeTrack;
using MusicApp.Application.Users.Commands.UpdateProfile;
using MusicApp.Application.Users.Queries.GetCurrentUser;

namespace MusicApp.API.Controllers;

[Authorize]
public class UsersController : ApiController
{
    public UsersController(ISender sender) : base(sender) { }

    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMe(CancellationToken ct)
        => Ok(await _sender.Send(new GetCurrentUserQuery(CurrentUserId), ct));

    [HttpPut("me")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateMe(
        [FromBody] UpdateProfileRequest request, CancellationToken ct)
    {
        await _sender.Send(
            new UpdateProfileCommand(CurrentUserId, request.DisplayName, request.AvatarUrl), ct);
        return NoContent();
    }

    [HttpPost("me/tracks/{trackId:guid}/like")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> LikeTrack(Guid trackId, CancellationToken ct)
    {
        await _sender.Send(new LikeTrackCommand(CurrentUserId, trackId), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/follow")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FollowUser(Guid id, CancellationToken ct)
    {
        await _sender.Send(new FollowUserCommand(CurrentUserId, id), ct);
        return NoContent();
    }
}

public record UpdateProfileRequest(string? DisplayName, string? AvatarUrl);
