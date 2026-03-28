using MediatR;
using MusicApp.Domain.Exceptions;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Application.Playlists.Commands.ReorderPlaylistTracks;

public class ReorderPlaylistTracksCommandHandler : IRequestHandler<ReorderPlaylistTracksCommand>
{
    private readonly IPlaylistRepository _playlistRepo;
    private readonly IUnitOfWork _uow;

    public ReorderPlaylistTracksCommandHandler(IPlaylistRepository playlistRepo, IUnitOfWork uow)
    { _playlistRepo = playlistRepo; _uow = uow; }

    public async Task Handle(ReorderPlaylistTracksCommand cmd, CancellationToken ct)
    {
        var playlist = await _playlistRepo.GetByIdWithTracksAsync(cmd.PlaylistId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Playlist), cmd.PlaylistId);
        playlist.ReorderTracks(cmd.TrackIds);
        await _uow.SaveChangesAsync(ct);
    }
}
