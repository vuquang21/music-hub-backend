using MediatR;

namespace MusicApp.Application.Albums.Commands.PublishAlbum;

public record PublishAlbumCommand(Guid Id) : IRequest;
