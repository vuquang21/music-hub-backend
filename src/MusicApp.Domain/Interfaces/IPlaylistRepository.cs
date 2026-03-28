using MusicApp.Domain.Entities;

namespace MusicApp.Domain.Interfaces;

public interface IPlaylistRepository
{
    Task<Playlist?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Playlist?> GetByIdWithTracksAsync(Guid id, CancellationToken ct = default);
    Task<PagedResult<Playlist>> GetPagedAsync(PlaylistFilter filter, CancellationToken ct = default);
    Task AddAsync(Playlist playlist, CancellationToken ct = default);
    void Update(Playlist playlist);
    void Remove(Playlist playlist);
}
