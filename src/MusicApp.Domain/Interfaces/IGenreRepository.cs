using MusicApp.Domain.Entities;

namespace MusicApp.Domain.Interfaces;

public interface IGenreRepository
{
    Task<Genre?> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task<List<Genre>> GetBySlugsAsync(List<string> slugs, CancellationToken ct = default);
    Task<List<Genre>> GetAllAsync(CancellationToken ct = default);
}
