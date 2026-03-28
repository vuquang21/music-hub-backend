using MusicApp.Domain.Entities;

namespace MusicApp.Domain.Interfaces;

public interface IArtistRepository
{
    Task<Artist?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Artist?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task AddAsync(Artist artist, CancellationToken ct = default);
    void Update(Artist artist);
}
