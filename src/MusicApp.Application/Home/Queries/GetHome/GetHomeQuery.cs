using MediatR;
using MusicApp.Application.Home.DTOs;

namespace MusicApp.Application.Home.Queries.GetHome;

/// <summary>
/// Returns all four home screen sections (Trending, For You, Discovery Weekly, Podcasts)
/// in a single request to minimise mobile round-trips.
/// </summary>
public record GetHomeQuery : IRequest<HomeDto>
{
    /// <summary>
    /// Maximum number of items to return per section.
    /// Defaults to 10. Clamped to [1, 20] by the handler.
    /// </summary>
    public int SectionLimit { get; init; } = 10;
}
