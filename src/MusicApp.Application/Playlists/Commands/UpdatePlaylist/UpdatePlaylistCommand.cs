using MediatR;

namespace MusicApp.Application.Playlists.Commands.UpdatePlaylist;

public record UpdatePlaylistCommand(Guid Id, string? Name = null, string? Description = null, bool? IsPublic = null, string? CoverImageUrl = null) : IRequest;
