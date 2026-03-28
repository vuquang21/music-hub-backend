using MediatR;
using MusicApp.Domain.Exceptions;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Application.Playlists.Commands.RemoveTrackFromPlaylist;

public class RemoveTrackFromPlaylistCommandHandler : IRequestHandler<RemoveTrackFromPlaylistCommand>
{
    private readonly IPlaylistRepository _playlistRepo;
    private readonly IUnitOfWork _uow;

    public RemoveTrackFromPlaylistCommandHandler(IPlaylistRepository playlistRepo, IUnitOfWork uow)
    { _playlistRepo = playlistRepo; _uow = uow; }

    public async Task Handle(RemoveTrackFromPlaylistCommand cmd, CancellationToken ct)
    {
        var playlist = await _playlistRepo.GetByIdWithTracksAsync(cmd.PlaylistId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Playlist), cmd.PlaylistId);
        playlist.RemoveTrack(cmd.TrackId);
        await _uow.SaveChangesAsync(ct);
    }
}
