using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicApp.Application.Common.DTOs;
using MusicApp.Application.Search.Commands.AddRecentSearch;
using MusicApp.Application.Search.Commands.DeleteRecentSearch;
using MusicApp.Application.Search.Queries.GetRecentSearches;
using MusicApp.Application.Search.Queries.SearchAll;

namespace MusicApp.API.Controllers;

[Authorize]
public class SearchController : ApiController
{
    public SearchController(ISender sender) : base(sender) { }

    /// <summary>Unified search across artists and tracks.</summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(
        [FromQuery] string q, [FromQuery] int limit = 20, CancellationToken ct = default)
        => Ok(await _sender.Send(new SearchAllQuery(q, limit), ct));

    /// <summary>Get the current user's recent search history.</summary>
    [HttpGet("recent")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecent(CancellationToken ct)
        => Ok(await _sender.Send(new GetRecentSearchesQuery(CurrentUserId), ct));

    /// <summary>Add a track to the current user's recent search history.</summary>
    [HttpPost("recent")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddRecent(
        [FromBody] AddRecentSearchRequest request, CancellationToken ct)
    {
        await _sender.Send(new AddRecentSearchCommand(CurrentUserId, request.TrackId), ct);
        return Ok<string?>(null, "Search history updated.");
    }

    /// <summary>Delete a specific entry from the current user's recent search history.</summary>
    [HttpDelete("recent/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteRecent(Guid id, CancellationToken ct)
    {
        await _sender.Send(new DeleteRecentSearchCommand(CurrentUserId, id), ct);
        return NoContent();
    }
}

public record AddRecentSearchRequest(Guid TrackId);
