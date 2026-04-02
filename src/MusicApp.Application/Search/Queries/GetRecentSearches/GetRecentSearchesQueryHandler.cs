using MediatR;
using MusicApp.Application.Search.DTOs;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Application.Search.Queries.GetRecentSearches;

public class GetRecentSearchesQueryHandler
    : IRequestHandler<GetRecentSearchesQuery, List<RecentSearchDto>>
{
    private readonly ISearchHistoryRepository _searchHistoryRepo;

    public GetRecentSearchesQueryHandler(ISearchHistoryRepository searchHistoryRepo)
        => _searchHistoryRepo = searchHistoryRepo;

    public async Task<List<RecentSearchDto>> Handle(
        GetRecentSearchesQuery request, CancellationToken ct)
    {
        var items = await _searchHistoryRepo.GetByUserIdAsync(request.UserId, 20, ct);

        return items.Select(sh => new RecentSearchDto(
            sh.Id,
            sh.TrackId,
            sh.Track.Title,
            sh.Track.Artist?.Name,
            sh.Track.Album?.CoverImageUrl,
            sh.SearchedAt
        )).ToList();
    }
}
