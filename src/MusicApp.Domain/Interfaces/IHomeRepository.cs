using MusicApp.Domain.Entities;

namespace MusicApp.Domain.Interfaces;

/// <summary>
/// Specialized read-only repository for home feed aggregation queries.
/// Each method is optimised for its specific section — do not use for generic CRUD.
/// </summary>
public interface IHomeRepository
{
    /// <summary>
    /// Returns the most-played published tracks from the last 30 days.
    /// </summary>
    Task<IReadOnlyList<Track>> GetTrendingTracksAsync(int limit, CancellationToken ct = default);

    /// <summary>
    /// Returns personalised tracks based on the authenticated user's liked genres and followed artists.
    /// Falls back to most-popular tracks for anonymous users or users with no history.
    /// </summary>
    Task<IReadOnlyList<Track>> GetForYouTracksAsync(Guid? userId, int limit, CancellationToken ct = default);

    /// <summary>
    /// Returns recently published tracks (7–14 days ago) that the user has not yet liked,
    /// ordered by play count.
    /// </summary>
    Task<IReadOnlyList<Track>> GetDiscoveryWeeklyTracksAsync(Guid? userId, int limit, CancellationToken ct = default);

    /// <summary>Returns active podcasts ordered by newest first.</summary>
    Task<IReadOnlyList<Podcast>> GetFeaturedPodcastsAsync(int limit, CancellationToken ct = default);
}
