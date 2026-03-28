using Microsoft.EntityFrameworkCore;
using MusicApp.Domain.Entities;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Infrastructure.Persistence.Repositories;

public class AlbumRepository : IAlbumRepository
{
    private readonly AppDbContext _context;
    public AlbumRepository(AppDbContext context) => _context = context;

    public async Task<Album?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _context.Albums.Include(a => a.Artist).Include(a => a.Tracks)
            .FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task<PagedResult<Album>> GetPagedAsync(AlbumFilter filter, CancellationToken ct)
    {
        var query = _context.Albums.Include(a => a.Artist).Include(a => a.Tracks).AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(a => a.Title.Contains(filter.Search));
        if (filter.ArtistId.HasValue)
            query = query.Where(a => a.ArtistId == filter.ArtistId.Value);

        query = filter.SortBy?.ToLower() switch
        {
            "title" => filter.SortDir == "asc" ? query.OrderBy(a => a.Title) : query.OrderByDescending(a => a.Title),
            _ => filter.SortDir == "asc" ? query.OrderBy(a => a.CreatedAt) : query.OrderByDescending(a => a.CreatedAt)
        };

        var total = await query.CountAsync(ct);
        var items = await query.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync(ct);
        return new PagedResult<Album>(items, total, filter.Page, filter.PageSize);
    }

    public async Task AddAsync(Album album, CancellationToken ct) => await _context.Albums.AddAsync(album, ct);
    public void Update(Album album) => _context.Albums.Update(album);
    public void Remove(Album album) => _context.Albums.Remove(album);
}
