using Microsoft.EntityFrameworkCore;
using MusicApp.Domain.Entities;
using MusicApp.Domain.Interfaces;
using MusicApp.Infrastructure.Persistence;

namespace MusicApp.Infrastructure.Persistence.Repositories;

/// <summary>
/// Optimised read-only queries for the home screen feed.
/// All methods use AsNoTracking and are tuned to avoid N+1 issues.
/// </summary>
public class HomeRepository : IHomeRepository
{
    private readonly AppDbContext _db;

    public HomeRepository(AppDbContext db) => _db = db;

    // -------------------------------------------------------------------------
    // Trending — top tracks by play count, published in the last 30 days
    // Index hint: (PlayCount DESC) on Tracks; query filter excludes Removed status
    // -------------------------------------------------------------------------
    public async Task<IReadOnlyList<Track>> GetTrendingTracksAsync(
        int limit, CancellationToken ct = default)
    {
        var cutoff = DateTime.UtcNow.AddDays(-30);

        return await _db.Tracks
            .AsNoTracking()
            .Include(t => t.Artist)
            .Include(t => t.Album)
            .Where(t => t.CreatedAt >= cutoff)
            .OrderByDescending(t => t.PlayCount)
            .Take(limit)
            .ToListAsync(ct);
    }

    // -------------------------------------------------------------------------
    // For You — personalised recommendations
    //   Auth users:  tracks from liked genres + followed artists, excl. liked
    //   Anonymous:   fallback to global most-popular
    //
    // Query plan:
    //   1. One projection query on Users to load preferences (liked genre/track IDs,
    //      followed user IDs) — avoids N+1 and loads only what we need.
    //   2. One point-lookup on Artists for followed user IDs.
    //   3. One track query with two EXISTS predicates (genre match OR artist match).
    // -------------------------------------------------------------------------
    public async Task<IReadOnlyList<Track>> GetForYouTracksAsync(
        Guid? userId, int limit, CancellationToken ct = default)
    {
        if (userId.HasValue)
        {
            var prefs = await _db.Users
                .AsNoTracking()
                .Where(u => u.Id == userId.Value)
                .Select(u => new
                {
                    LikedTrackIds = u.LikedTracks.Select(t => t.Id).ToList(),
                    LikedGenreIds = u.LikedTracks
                        .SelectMany(t => t.Genres)
                        .Select(g => g.Id)
                        .Distinct()
                        .ToList(),
                    FollowedUserIds = u.Following.Select(f => f.Id).ToList()
                })
                .FirstOrDefaultAsync(ct);

            if (prefs is not null && (prefs.LikedGenreIds.Count > 0 || prefs.FollowedUserIds.Count > 0))
            {
                var followedArtistIds = await _db.Artists
                    .AsNoTracking()
                    .Where(a => prefs.FollowedUserIds.Contains(a.UserId))
                    .Select(a => a.Id)
                    .ToListAsync(ct);

                var personalised = await _db.Tracks
                    .AsNoTracking()
                    .Include(t => t.Artist)
                    .Include(t => t.Album)
                    .Where(t =>
                        !prefs.LikedTrackIds.Contains(t.Id) &&
                        (t.Genres.Any(g => prefs.LikedGenreIds.Contains(g.Id)) ||
                         followedArtistIds.Contains(t.ArtistId)))
                    .OrderByDescending(t => t.PlayCount)
                    .Take(limit)
                    .ToListAsync(ct);

                if (personalised.Count > 0)
                    return personalised;
            }
        }

        // Fallback: global most-popular (used for anonymous + users with no history)
        return await _db.Tracks
            .AsNoTracking()
            .Include(t => t.Artist)
            .Include(t => t.Album)
            .OrderByDescending(t => t.PlayCount)
            .Take(limit)
            .ToListAsync(ct);
    }

    // -------------------------------------------------------------------------
    // Discovery Weekly — tracks published 7–14 days ago that the user hasn't liked.
    // The "7–14 days" window rotates automatically each week without any scheduled job.
    // -------------------------------------------------------------------------
    public async Task<IReadOnlyList<Track>> GetDiscoveryWeeklyTracksAsync(
        Guid? userId, int limit, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var weekStart = now.AddDays(-14);
        var weekEnd = now.AddDays(-7);

        var query = _db.Tracks
            .AsNoTracking()
            .Include(t => t.Artist)
            .Include(t => t.Album)
            .Where(t => t.CreatedAt >= weekStart && t.CreatedAt < weekEnd);

        if (userId.HasValue)
        {
            var likedIds = await _db.Users
                .AsNoTracking()
                .Where(u => u.Id == userId.Value)
                .SelectMany(u => u.LikedTracks)
                .Select(t => t.Id)
                .ToListAsync(ct);

            query = query.Where(t => !likedIds.Contains(t.Id));
        }

        return await query
            .OrderByDescending(t => t.PlayCount)
            .Take(limit)
            .ToListAsync(ct);
    }

    // -------------------------------------------------------------------------
    // Podcasts — active podcasts, newest first
    // -------------------------------------------------------------------------
    public async Task<IReadOnlyList<Podcast>> GetFeaturedPodcastsAsync(
        int limit, CancellationToken ct = default)
    {
        return await _db.Podcasts
            .AsNoTracking()
            .Where(p => p.IsActive)
            .OrderByDescending(p => p.CreatedAt)
            .Take(limit)
            .ToListAsync(ct);
    }
}
