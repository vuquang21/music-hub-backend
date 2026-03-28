using MediatR;

namespace MusicApp.Application.Albums.Commands.UpdateAlbum;

public record UpdateAlbumCommand(Guid Id, string? Title, string? CoverImageUrl, int? ReleaseYear) : IRequest;
