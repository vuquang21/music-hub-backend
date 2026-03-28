using MediatR;

namespace MusicApp.Application.Playlists.Commands.RemoveTrackFromPlaylist;

public record RemoveTrackFromPlaylistCommand(Guid PlaylistId, Guid TrackId) : IRequest;
