using Microsoft.EntityFrameworkCore;
using MusicApp.Domain.Entities;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Infrastructure.Persistence.Repositories;

public class PlaylistRepository : IPlaylistRepository
{
    private readonly AppDbContext _context;
    public PlaylistRepository(AppDbContext context) => _context = context;

    public async Task<Playlist?> GetByIdAsync(Guid id, CancellationToken ct)
        => await _context.Playlists.Include(p => p.Owner).Include(p => p.PlaylistTracks).Include(p => p.Followers)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<Playlist?> GetByIdWithTracksAsync(Guid id, CancellationToken ct)
        => await _context.Playlists.Include(p => p.PlaylistTracks).ThenInclude(pt => pt.Track)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<PagedResult<Playlist>> GetPagedAsync(PlaylistFilter filter, CancellationToken ct)
    {
        var query = _context.Playlists.Include(p => p.Owner).Include(p => p.PlaylistTracks).Include(p => p.Followers).AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(p => p.Name.Contains(filter.Search));
        if (filter.OwnerId.HasValue)
            query = query.Where(p => p.OwnerId == filter.OwnerId.Value);
        if (filter.IsPublic.HasValue)
            query = query.Where(p => p.IsPublic == filter.IsPublic.Value);

        query = filter.SortBy?.ToLower() switch
        {
            "name" => filter.SortDir == "asc" ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name),
            _ => filter.SortDir == "asc" ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt)
        };

        var total = await query.CountAsync(ct);
        var items = await query.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync(ct);
        return new PagedResult<Playlist>(items, total, filter.Page, filter.PageSize);
    }

    public async Task AddAsync(Playlist playlist, CancellationToken ct) => await _context.Playlists.AddAsync(playlist, ct);
    public void Update(Playlist playlist) => _context.Playlists.Update(playlist);
    public void Remove(Playlist playlist) => _context.Playlists.Remove(playlist);
}
