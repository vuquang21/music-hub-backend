using MediatR;

namespace MusicApp.Application.Playlists.Commands.AddTrackToPlaylist;

public record AddTrackToPlaylistCommand(Guid PlaylistId, Guid TrackId) : IRequest;
