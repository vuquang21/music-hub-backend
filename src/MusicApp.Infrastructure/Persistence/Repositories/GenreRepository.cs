using Microsoft.EntityFrameworkCore;
using MusicApp.Domain.Entities;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Infrastructure.Persistence.Repositories;

public class GenreRepository : IGenreRepository
{
    private readonly AppDbContext _context;
    public GenreRepository(AppDbContext context) => _context = context;

    public async Task<Genre?> GetBySlugAsync(string slug, CancellationToken ct)
        => await _context.Genres.FirstOrDefaultAsync(g => g.Slug == slug, ct);

    public async Task<List<Genre>> GetBySlugsAsync(List<string> slugs, CancellationToken ct)
        => await _context.Genres.Where(g => slugs.Contains(g.Slug)).ToListAsync(ct);

    public async Task<List<Genre>> GetAllAsync(CancellationToken ct)
        => await _context.Genres.OrderBy(g => g.Name).ToListAsync(ct);
}
