using MediatR;

namespace MusicApp.Application.Playlists.Commands.DeletePlaylist;

public record DeletePlaylistCommand(Guid Id) : IRequest;
