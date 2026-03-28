using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicApp.Application.Common.DTOs;
using MusicApp.Application.Playlists.Commands.AddTrackToPlaylist;
using MusicApp.Application.Playlists.Commands.CreatePlaylist;
using MusicApp.Application.Playlists.Commands.DeletePlaylist;
using MusicApp.Application.Playlists.Commands.RemoveTrackFromPlaylist;
using MusicApp.Application.Playlists.Commands.ReorderPlaylistTracks;
using MusicApp.Application.Playlists.Commands.UpdatePlaylist;
using MusicApp.Application.Playlists.Queries.GetPlaylistById;
using MusicApp.Application.Playlists.Queries.GetPlaylistsPaged;

namespace MusicApp.API.Controllers;

[Authorize]
public class PlaylistsController : ApiController
{
    public PlaylistsController(ISender sender) : base(sender) { }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetPlaylistsPagedQuery query, CancellationToken ct)
    {
        var result = await _sender.Send(query, ct);
        return Paged(result.Items, result.TotalCount, result.Page, result.PageSize);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await _sender.Send(new GetPlaylistByIdQuery(id), ct));

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreatePlaylistRequest request, CancellationToken ct)
    {
        var id = await _sender.Send(
            new CreatePlaylistCommand(request.Name, CurrentUserId, request.Description, request.IsPublic), ct);
        return Created(new { id }, $"/api/v1/playlists/{id}");
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id,
        [FromBody] UpdatePlaylistRequest request, CancellationToken ct)
    {
        await _sender.Send(
            new UpdatePlaylistCommand(id, request.Name, request.Description, request.IsPublic, request.CoverImageUrl), ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _sender.Send(new DeletePlaylistCommand(id), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/tracks")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddTrack(Guid id,
        [FromBody] AddTrackRequest request, CancellationToken ct)
    {
        await _sender.Send(new AddTrackToPlaylistCommand(id, request.TrackId), ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}/tracks/{trackId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveTrack(Guid id, Guid trackId, CancellationToken ct)
    {
        await _sender.Send(new RemoveTrackFromPlaylistCommand(id, trackId), ct);
        return NoContent();
    }

    [HttpPut("{id:guid}/tracks/reorder")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReorderTracks(Guid id,
        [FromBody] ReorderTracksRequest request, CancellationToken ct)
    {
        await _sender.Send(new ReorderPlaylistTracksCommand(id, request.TrackIds), ct);
        return NoContent();
    }
}

public record CreatePlaylistRequest(string Name, string? Description = null, bool IsPublic = true);
public record UpdatePlaylistRequest(string? Name = null, string? Description = null, bool? IsPublic = null, string? CoverImageUrl = null);
public record AddTrackRequest(Guid TrackId);
public record ReorderTracksRequest(List<Guid> TrackIds);
