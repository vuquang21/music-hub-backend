using MediatR;
using MusicApp.Domain.Exceptions;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Application.Playlists.Commands.AddTrackToPlaylist;

public class AddTrackToPlaylistCommandHandler : IRequestHandler<AddTrackToPlaylistCommand>
{
    private readonly IPlaylistRepository _playlistRepo;
    private readonly IUnitOfWork _uow;

    public AddTrackToPlaylistCommandHandler(IPlaylistRepository playlistRepo, IUnitOfWork uow)
    { _playlistRepo = playlistRepo; _uow = uow; }

    public async Task Handle(AddTrackToPlaylistCommand cmd, CancellationToken ct)
    {
        var playlist = await _playlistRepo.GetByIdWithTracksAsync(cmd.PlaylistId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Playlist), cmd.PlaylistId);
        playlist.AddTrack(cmd.TrackId);
        await _uow.SaveChangesAsync(ct);
    }
}
