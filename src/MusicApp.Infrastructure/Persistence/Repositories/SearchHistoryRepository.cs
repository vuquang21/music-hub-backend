using Microsoft.EntityFrameworkCore;
using MusicApp.Domain.Entities;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Infrastructure.Persistence.Repositories;

public class SearchHistoryRepository : ISearchHistoryRepository
{
    private readonly AppDbContext _context;
    public SearchHistoryRepository(AppDbContext context) => _context = context;

    public async Task<List<SearchHistory>> GetByUserIdAsync(
        Guid userId, int limit = 20, CancellationToken ct = default)
        => await _context.SearchHistories
            .Include(sh => sh.Track)
                .ThenInclude(t => t.Artist)
            .Include(sh => sh.Track)
                .ThenInclude(t => t.Album)
            .Where(sh => sh.UserId == userId)
            .OrderByDescending(sh => sh.SearchedAt)
            .Take(limit)
            .ToListAsync(ct);

    public async Task<SearchHistory?> GetByUserAndTrackAsync(
        Guid userId, Guid trackId, CancellationToken ct = default)
        => await _context.SearchHistories
            .FirstOrDefaultAsync(sh => sh.UserId == userId && sh.TrackId == trackId, ct);

    public async Task<SearchHistory?> GetByIdAndUserAsync(
        Guid id, Guid userId, CancellationToken ct = default)
        => await _context.SearchHistories
            .FirstOrDefaultAsync(sh => sh.Id == id && sh.UserId == userId, ct);

    public async Task AddAsync(SearchHistory searchHistory, CancellationToken ct = default)
        => await _context.SearchHistories.AddAsync(searchHistory, ct);

    public void Remove(SearchHistory searchHistory)
        => _context.SearchHistories.Remove(searchHistory);
}
