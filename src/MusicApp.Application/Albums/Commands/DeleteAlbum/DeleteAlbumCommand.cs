using MediatR;

namespace MusicApp.Application.Albums.Commands.DeleteAlbum;

public record DeleteAlbumCommand(Guid Id) : IRequest;
