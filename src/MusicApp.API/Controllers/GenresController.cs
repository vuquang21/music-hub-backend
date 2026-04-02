using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicApp.Application.Common.DTOs;
using MusicApp.Application.Genres.Queries.GetAllGenres;

namespace MusicApp.API.Controllers;

[Authorize]
public class GenresController : ApiController
{
    public GenresController(ISender sender) : base(sender) { }

    /// <summary>Get all available genres.</summary>
    [HttpGet]
    [AllowAnonymous]
    [ResponseCache(Duration = 300)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await _sender.Send(new GetAllGenresQuery(), ct));
}
