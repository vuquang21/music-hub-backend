using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicApp.Application.Albums.Commands.CreateAlbum;
using MusicApp.Application.Albums.Commands.DeleteAlbum;
using MusicApp.Application.Albums.Commands.PublishAlbum;
using MusicApp.Application.Albums.Commands.UpdateAlbum;
using MusicApp.Application.Albums.Queries.GetAlbumById;
using MusicApp.Application.Albums.Queries.GetAlbumsPaged;
using MusicApp.Application.Common.DTOs;

namespace MusicApp.API.Controllers;

[Authorize]
public class AlbumsController : ApiController
{
    public AlbumsController(ISender sender) : base(sender) { }

    [HttpGet]
    [AllowAnonymous]
    [ResponseCache(Duration = 60)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetAlbumsPagedQuery query, CancellationToken ct)
    {
        var result = await _sender.Send(query, ct);
        return Paged(result.Items, result.TotalCount, result.Page, result.PageSize);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await _sender.Send(new GetAlbumByIdQuery(id), ct));

    [HttpPost]
    [Authorize(Roles = "Artist,Admin")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateAlbumCommand cmd, CancellationToken ct)
    {
        var id = await _sender.Send(cmd, ct);
        return Created(new { id }, $"/api/v1/albums/{id}");
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Artist,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id,
        [FromBody] UpdateAlbumRequest request, CancellationToken ct)
    {
        await _sender.Send(new UpdateAlbumCommand(id, request.Title, request.CoverImageUrl, request.ReleaseYear), ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/publish")]
    [Authorize(Roles = "Artist,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Publish(Guid id, CancellationToken ct)
    {
        await _sender.Send(new PublishAlbumCommand(id), ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Artist,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _sender.Send(new DeleteAlbumCommand(id), ct);
        return NoContent();
    }
}

public record UpdateAlbumRequest(string? Title, string? CoverImageUrl, int? ReleaseYear);
