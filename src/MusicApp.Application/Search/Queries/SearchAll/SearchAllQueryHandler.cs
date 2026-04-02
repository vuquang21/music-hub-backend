using MediatR;
using MusicApp.Application.Search.DTOs;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Application.Search.Queries.SearchAll;

public class SearchAllQueryHandler : IRequestHandler<SearchAllQuery, SearchResultDto>
{
    private readonly ISearchRepository _searchRepo;

    public SearchAllQueryHandler(ISearchRepository searchRepo)
        => _searchRepo = searchRepo;

    public async Task<SearchResultDto> Handle(SearchAllQuery request, CancellationToken ct)
    {
        var q = request.Q.Trim();
        var limit = request.Limit;

        var artists = await _searchRepo.SearchArtistsAsync(q, limit, ct);
        var tracks = await _searchRepo.SearchTracksAsync(q, limit, ct);

        var items = new List<SearchResultItemDto>();

        items.AddRange(artists.Select(a => new SearchResultItemDto(
            a.Id, "artist", a.Name, "Artist", a.ImageUrl)));

        items.AddRange(tracks.Select(t => new SearchResultItemDto(
            t.Id, "track", t.Title, t.Artist?.Name, t.Album?.CoverImageUrl)));

        return new SearchResultDto(items);
    }
}
