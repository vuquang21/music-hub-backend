using AutoMapper;
using MediatR;
using MusicApp.Application.Tracks.DTOs;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Application.Tracks.Queries.GetTracksPaged;

public class GetTracksPagedQueryHandler : IRequestHandler<GetTracksPagedQuery, PagedResult<TrackDto>>
{
    private readonly ITrackRepository _trackRepo;
    private readonly IMapper _mapper;

    public GetTracksPagedQueryHandler(ITrackRepository trackRepo, IMapper mapper)
    { _trackRepo = trackRepo; _mapper = mapper; }

    public async Task<PagedResult<TrackDto>> Handle(GetTracksPagedQuery q, CancellationToken ct)
    {
        var filter = new TrackFilter
        {
            Page = q.Page, PageSize = Math.Min(q.PageSize, 100),
            Search = q.Search, Genre = q.Genre, ArtistId = q.ArtistId,
            SortBy = q.SortBy, SortDir = q.SortDir, From = q.From, To = q.To
        };
        var result = await _trackRepo.GetPagedAsync(filter, ct);
        var dtos = _mapper.Map<List<TrackDto>>(result.Items);
        return new PagedResult<TrackDto>(dtos, result.TotalCount, result.Page, result.PageSize);
    }
}
