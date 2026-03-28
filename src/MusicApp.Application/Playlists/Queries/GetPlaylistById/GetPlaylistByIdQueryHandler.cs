using AutoMapper;
using MediatR;
using MusicApp.Application.Playlists.DTOs;
using MusicApp.Domain.Exceptions;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Application.Playlists.Queries.GetPlaylistById;

public class GetPlaylistByIdQueryHandler : IRequestHandler<GetPlaylistByIdQuery, PlaylistDto>
{
    private readonly IPlaylistRepository _playlistRepo;
    private readonly IMapper _mapper;

    public GetPlaylistByIdQueryHandler(IPlaylistRepository playlistRepo, IMapper mapper)
    { _playlistRepo = playlistRepo; _mapper = mapper; }

    public async Task<PlaylistDto> Handle(GetPlaylistByIdQuery q, CancellationToken ct)
    {
        var playlist = await _playlistRepo.GetByIdAsync(q.Id, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Playlist), q.Id);
        return _mapper.Map<PlaylistDto>(playlist);
    }
}
