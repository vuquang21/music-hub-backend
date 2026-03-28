using MediatR;

namespace MusicApp.Application.Albums.Commands.CreateAlbum;

public record CreateAlbumCommand(string Title, Guid ArtistId, int? ReleaseYear = null) : IRequest<Guid>;
