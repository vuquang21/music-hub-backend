using MediatR;
using MusicApp.Domain.Exceptions;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Application.Playlists.Commands.UpdatePlaylist;

public class UpdatePlaylistCommandHandler : IRequestHandler<UpdatePlaylistCommand>
{
    private readonly IPlaylistRepository _playlistRepo;
    private readonly IUnitOfWork _uow;

    public UpdatePlaylistCommandHandler(IPlaylistRepository playlistRepo, IUnitOfWork uow)
    { _playlistRepo = playlistRepo; _uow = uow; }

    public async Task Handle(UpdatePlaylistCommand cmd, CancellationToken ct)
    {
        var playlist = await _playlistRepo.GetByIdAsync(cmd.Id, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Playlist), cmd.Id);
        playlist.Update(cmd.Name, cmd.Description, cmd.IsPublic, cmd.CoverImageUrl);
        await _uow.SaveChangesAsync(ct);
    }
}
