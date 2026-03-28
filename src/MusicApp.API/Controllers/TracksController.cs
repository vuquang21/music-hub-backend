using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicApp.Application.Common.DTOs;
using MusicApp.Application.Tracks.Commands.CreateTrack;
using MusicApp.Application.Tracks.Commands.DeleteTrack;
using MusicApp.Application.Tracks.Commands.PublishTrack;
using MusicApp.Application.Tracks.Commands.RecordPlay;
using MusicApp.Application.Tracks.Commands.UpdateTrack;
using MusicApp.Application.Tracks.Queries.GetTrackById;
using MusicApp.Application.Tracks.Queries.GetTracksPaged;

namespace MusicApp.API.Controllers;

[Authorize]
public class TracksController : ApiController
{
    public TracksController(ISender sender) : base(sender) { }

    [HttpGet]
    [AllowAnonymous]
    [ResponseCache(Duration = 60)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetTracksPagedQuery query, CancellationToken ct)
    {
        var result = await _sender.Send(query, ct);
        return Paged(result.Items, result.TotalCount, result.Page, result.PageSize);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await _sender.Send(new GetTrackByIdQuery(id), ct));

    [HttpPost]
    [Authorize(Roles = "Artist,Admin")]
    [RequestSizeLimit(105_000_000)]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromForm] CreateTrackCommand cmd, CancellationToken ct)
    {
        var id = await _sender.Send(cmd, ct);
        return Created(new { id }, $"/api/v1/tracks/{id}");
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Artist,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id,
        [FromBody] UpdateTrackRequest request, CancellationToken ct)
    {
        await _sender.Send(new UpdateTrackCommand(id, request.Title, request.AlbumId), ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/publish")]
    [Authorize(Roles = "Artist,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Publish(Guid id, CancellationToken ct)
    {
        await _sender.Send(new PublishTrackCommand(id), ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Artist,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _sender.Send(new DeleteTrackCommand(id), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/play")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Play(Guid id, CancellationToken ct)
        => Ok(await _sender.Send(new RecordPlayCommand(id, CurrentUserId), ct));
}

public record UpdateTrackRequest(string? Title, Guid? AlbumId);
