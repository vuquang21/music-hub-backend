using MusicApp.Domain.Entities;

namespace MusicApp.Domain.Interfaces;

/// <summary>CRUD repository for the <see cref="Podcast"/> aggregate.</summary>
public interface IPodcastRepository
{
    Task<Podcast?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Podcast>> GetAllActiveAsync(CancellationToken ct = default);
    Task AddAsync(Podcast podcast, CancellationToken ct = default);
    void Update(Podcast podcast);
    void Remove(Podcast podcast);
}
