using AutoMapper;
using MediatR;
using MusicApp.Application.Albums.DTOs;
using MusicApp.Domain.Exceptions;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Application.Albums.Queries.GetAlbumById;

public class GetAlbumByIdQueryHandler : IRequestHandler<GetAlbumByIdQuery, AlbumDto>
{
    private readonly IAlbumRepository _albumRepo;
    private readonly IMapper _mapper;

    public GetAlbumByIdQueryHandler(IAlbumRepository albumRepo, IMapper mapper)
    { _albumRepo = albumRepo; _mapper = mapper; }

    public async Task<AlbumDto> Handle(GetAlbumByIdQuery q, CancellationToken ct)
    {
        var album = await _albumRepo.GetByIdAsync(q.Id, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Album), q.Id);
        return _mapper.Map<AlbumDto>(album);
    }
}
