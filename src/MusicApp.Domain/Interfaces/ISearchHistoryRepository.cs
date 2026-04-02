using MusicApp.Domain.Entities;

namespace MusicApp.Domain.Interfaces;

public interface ISearchHistoryRepository
{
    Task<List<SearchHistory>> GetByUserIdAsync(Guid userId, int limit = 20, CancellationToken ct = default);
    Task<SearchHistory?> GetByUserAndTrackAsync(Guid userId, Guid trackId, CancellationToken ct = default);
    Task<SearchHistory?> GetByIdAndUserAsync(Guid id, Guid userId, CancellationToken ct = default);
    Task AddAsync(SearchHistory searchHistory, CancellationToken ct = default);
    void Remove(SearchHistory searchHistory);
}
