using Microsoft.EntityFrameworkCore;
using MusicApp.Domain.Entities;
using MusicApp.Domain.Interfaces;
using MusicApp.Infrastructure.Persistence;

namespace MusicApp.Infrastructure.Persistence.Repositories;

public class PodcastRepository : IPodcastRepository
{
    private readonly AppDbContext _db;

    public PodcastRepository(AppDbContext db) => _db = db;

    public async Task<Podcast?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Podcasts.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<IReadOnlyList<Podcast>> GetAllActiveAsync(CancellationToken ct = default)
        => await _db.Podcasts
            .AsNoTracking()
            .Where(p => p.IsActive)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(ct);

    public async Task AddAsync(Podcast podcast, CancellationToken ct = default)
        => await _db.Podcasts.AddAsync(podcast, ct);

    public void Update(Podcast podcast)
        => _db.Podcasts.Update(podcast);

    public void Remove(Podcast podcast)
        => _db.Podcasts.Remove(podcast);
}
