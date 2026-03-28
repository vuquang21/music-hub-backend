using MediatR;

namespace MusicApp.Application.Playlists.Commands.ReorderPlaylistTracks;

public record ReorderPlaylistTracksCommand(Guid PlaylistId, List<Guid> TrackIds) : IRequest;
