using MediatR;
using MusicApp.Domain.Entities;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Application.Playlists.Commands.CreatePlaylist;

public class CreatePlaylistCommandHandler : IRequestHandler<CreatePlaylistCommand, Guid>
{
    private readonly IPlaylistRepository _playlistRepo;
    private readonly IUnitOfWork _uow;

    public CreatePlaylistCommandHandler(IPlaylistRepository playlistRepo, IUnitOfWork uow)
    { _playlistRepo = playlistRepo; _uow = uow; }

    public async Task<Guid> Handle(CreatePlaylistCommand cmd, CancellationToken ct)
    {
        var playlist = Playlist.Create(cmd.Name, cmd.OwnerId, cmd.Description, cmd.IsPublic);
        await _playlistRepo.AddAsync(playlist, ct);
        await _uow.SaveChangesAsync(ct);
        return playlist.Id;
    }
}
