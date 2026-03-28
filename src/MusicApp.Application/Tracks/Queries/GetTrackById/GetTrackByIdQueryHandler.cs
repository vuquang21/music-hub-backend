using AutoMapper;
using MediatR;
using MusicApp.Application.Tracks.DTOs;
using MusicApp.Domain.Exceptions;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Application.Tracks.Queries.GetTrackById;

public class GetTrackByIdQueryHandler : IRequestHandler<GetTrackByIdQuery, TrackDto>
{
    private readonly ITrackRepository _trackRepo;
    private readonly IMapper _mapper;

    public GetTrackByIdQueryHandler(ITrackRepository trackRepo, IMapper mapper)
    { _trackRepo = trackRepo; _mapper = mapper; }

    public async Task<TrackDto> Handle(GetTrackByIdQuery q, CancellationToken ct)
    {
        var track = await _trackRepo.GetByIdAsync(q.Id, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Track), q.Id);
        return _mapper.Map<TrackDto>(track);
    }
}
