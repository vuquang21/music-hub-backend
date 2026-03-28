using MediatR;
using MusicApp.Application.Albums.DTOs;

namespace MusicApp.Application.Albums.Queries.GetAlbumById;

public record GetAlbumByIdQuery(Guid Id) : IRequest<AlbumDto>;
