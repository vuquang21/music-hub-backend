using MusicApp.Domain.Entities;

namespace MusicApp.Domain.Interfaces;

public interface ITrackRepository
{
    Task<Track?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Track?> GetByIsrcAsync(string isrc, CancellationToken ct = default);
    Task<PagedResult<Track>> GetPagedAsync(TrackFilter filter, CancellationToken ct = default);
    Task AddAsync(Track track, CancellationToken ct = default);
    void Update(Track track);
    void Remove(Track track);
}
