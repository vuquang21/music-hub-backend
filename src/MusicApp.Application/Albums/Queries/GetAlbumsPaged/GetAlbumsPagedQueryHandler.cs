using AutoMapper;
using MediatR;
using MusicApp.Application.Albums.DTOs;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Application.Albums.Queries.GetAlbumsPaged;

public class GetAlbumsPagedQueryHandler : IRequestHandler<GetAlbumsPagedQuery, PagedResult<AlbumDto>>
{
    private readonly IAlbumRepository _albumRepo;
    private readonly IMapper _mapper;

    public GetAlbumsPagedQueryHandler(IAlbumRepository albumRepo, IMapper mapper)
    { _albumRepo = albumRepo; _mapper = mapper; }

    public async Task<PagedResult<AlbumDto>> Handle(GetAlbumsPagedQuery q, CancellationToken ct)
    {
        var filter = new AlbumFilter
        {
            Page = q.Page, PageSize = Math.Min(q.PageSize, 100),
            Search = q.Search, ArtistId = q.ArtistId, SortBy = q.SortBy, SortDir = q.SortDir
        };
        var result = await _albumRepo.GetPagedAsync(filter, ct);
        var dtos = _mapper.Map<List<AlbumDto>>(result.Items);
        return new PagedResult<AlbumDto>(dtos, result.TotalCount, result.Page, result.PageSize);
    }
}
