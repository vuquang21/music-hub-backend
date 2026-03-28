using Microsoft.EntityFrameworkCore;
using MusicApp.Domain.Entities;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Infrastructure.Persistence.Repositories;

public class TrackRepository : ITrackRepository
{
    private readonly AppDbContext _context;
    public TrackRepository(AppDbContext context) => _context = context;

    public async Task<Track?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _context.Tracks.Include(t => t.Artist).Include(t => t.Genres)
            .FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<Track?> GetByIsrcAsync(string isrc, CancellationToken ct)
        => await _context.Tracks.FirstOrDefaultAsync(t => EF.Property<string>(t, "Isrc") == isrc, ct);

    public async Task<PagedResult<Track>> GetPagedAsync(TrackFilter filter, CancellationToken ct)
    {
        var query = _context.Tracks.Include(t => t.Artist).Include(t => t.Genres).AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(t => t.Title.Contains(filter.Search));
        if (filter.Genre is not null)
            query = query.Where(t => t.Genres.Any(g => g.Slug == filter.Genre));
        if (filter.ArtistId.HasValue)
            query = query.Where(t => t.ArtistId == filter.ArtistId.Value);
        if (filter.From.HasValue)
            query = query.Where(t => t.CreatedAt >= filter.From.Value);
        if (filter.To.HasValue)
            query = query.Where(t => t.CreatedAt <= filter.To.Value);

        query = filter.SortBy?.ToLower() switch
        {
            "title" => filter.SortDir == "asc" ? query.OrderBy(t => t.Title) : query.OrderByDescending(t => t.Title),
            "createdat" => filter.SortDir == "asc" ? query.OrderBy(t => t.CreatedAt) : query.OrderByDescending(t => t.CreatedAt),
            _ => filter.SortDir == "asc" ? query.OrderBy(t => t.PlayCount) : query.OrderByDescending(t => t.PlayCount)
        };

        var total = await query.CountAsync(ct);
        var items = await query.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync(ct);
        return new PagedResult<Track>(items, total, filter.Page, filter.PageSize);
    }

    public async Task AddAsync(Track track, CancellationToken ct) => await _context.Tracks.AddAsync(track, ct);
    public void Update(Track track) => _context.Tracks.Update(track);
    public void Remove(Track track) => _context.Tracks.Remove(track);
}
