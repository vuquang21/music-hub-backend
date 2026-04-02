using MusicApp.Domain.Entities;

namespace MusicApp.Domain.Interfaces;

public interface ISearchRepository
{
    Task<List<Artist>> SearchArtistsAsync(string query, int limit, CancellationToken ct = default);
    Task<List<Track>> SearchTracksAsync(string query, int limit, CancellationToken ct = default);
}
