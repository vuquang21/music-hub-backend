using Microsoft.EntityFrameworkCore;
using MusicApp.Domain.Entities;
using MusicApp.Domain.Enums;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Infrastructure.Persistence.Repositories;

public class SearchRepository : ISearchRepository
{
    private readonly AppDbContext _context;
    public SearchRepository(AppDbContext context) => _context = context;

    public async Task<List<Artist>> SearchArtistsAsync(
        string query, int limit, CancellationToken ct = default)
        => await _context.Artists
            .Where(a => EF.Functions.ILike(a.Name, $"%{query}%"))
            .OrderBy(a => a.Name)
            .Take(limit)
            .ToListAsync(ct);

    public async Task<List<Track>> SearchTracksAsync(
        string query, int limit, CancellationToken ct = default)
        => await _context.Tracks
            .Include(t => t.Artist)
            .Include(t => t.Album)
            .Where(t => t.Status == TrackStatus.Published
                && (EF.Functions.ILike(t.Title, $"%{query}%")
                    || EF.Functions.ILike(t.Artist.Name, $"%{query}%")))
            .OrderBy(t => t.Title)
            .Take(limit)
            .ToListAsync(ct);
}
