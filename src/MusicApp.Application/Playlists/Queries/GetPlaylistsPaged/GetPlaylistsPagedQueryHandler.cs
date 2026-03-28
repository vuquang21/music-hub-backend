using AutoMapper;
using MediatR;
using MusicApp.Application.Playlists.DTOs;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Application.Playlists.Queries.GetPlaylistsPaged;

public class GetPlaylistsPagedQueryHandler : IRequestHandler<GetPlaylistsPagedQuery, PagedResult<PlaylistDto>>
{
    private readonly IPlaylistRepository _playlistRepo;
    private readonly IMapper _mapper;

    public GetPlaylistsPagedQueryHandler(IPlaylistRepository playlistRepo, IMapper mapper)
    { _playlistRepo = playlistRepo; _mapper = mapper; }

    public async Task<PagedResult<PlaylistDto>> Handle(GetPlaylistsPagedQuery q, CancellationToken ct)
    {
        var filter = new PlaylistFilter
        {
            Page = q.Page, PageSize = Math.Min(q.PageSize, 100),
            Search = q.Search, OwnerId = q.OwnerId, IsPublic = q.IsPublic,
            SortBy = q.SortBy, SortDir = q.SortDir
        };
        var result = await _playlistRepo.GetPagedAsync(filter, ct);
        var dtos = _mapper.Map<List<PlaylistDto>>(result.Items);
        return new PagedResult<PlaylistDto>(dtos, result.TotalCount, result.Page, result.PageSize);
    }
}
