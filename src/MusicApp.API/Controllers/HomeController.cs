using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicApp.Application.Home.Queries.GetHome;

namespace MusicApp.API.Controllers;

/// <summary>
/// Returns all home screen sections in a single request to minimise mobile round-trips.
/// </summary>
public class HomeController : ApiController
{
    public HomeController(ISender sender) : base(sender) { }

    /// <summary>
    /// GET /api/v1/home
    ///
    /// Returns four sections: Trending, For You, Discovery Weekly, and Podcasts.
    /// Authentication is optional — personalised sections (For You, Discovery Weekly)
    /// degrade gracefully to popular/recent content for anonymous users.
    /// </summary>
    /// <param name="sectionLimit">Items per section (1–20, default 10).</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Home feed payload.</response>
    /// <response code="400">Invalid sectionLimit value.</response>
    [HttpGet]
    [AllowAnonymous]
    [ResponseCache(Duration = 30, VaryByQueryKeys = new[] { "sectionLimit" })]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetHome(
        [FromQuery] int sectionLimit = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(
            new GetHomeQuery { SectionLimit = sectionLimit },
            cancellationToken);

        return Ok(result);
    }
}
