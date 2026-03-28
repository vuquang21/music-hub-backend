using MediatR;

namespace MusicApp.Application.Playlists.Commands.CreatePlaylist;

public record CreatePlaylistCommand(string Name, Guid OwnerId, string? Description = null, bool IsPublic = true) : IRequest<Guid>;
