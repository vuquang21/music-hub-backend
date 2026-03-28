using MusicApp.Domain.Entities;

namespace MusicApp.Domain.Interfaces;

public interface IAlbumRepository
{
    Task<Album?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PagedResult<Album>> GetPagedAsync(AlbumFilter filter, CancellationToken ct = default);
    Task AddAsync(Album album, CancellationToken ct = default);
    void Update(Album album);
    void Remove(Album album);
}
