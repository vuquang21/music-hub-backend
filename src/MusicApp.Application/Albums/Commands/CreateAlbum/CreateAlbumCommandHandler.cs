using MediatR;
using MusicApp.Domain.Entities;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Application.Albums.Commands.CreateAlbum;

public class CreateAlbumCommandHandler : IRequestHandler<CreateAlbumCommand, Guid>
{
    private readonly IAlbumRepository _albumRepo;
    private readonly IUnitOfWork _uow;

    public CreateAlbumCommandHandler(IAlbumRepository albumRepo, IUnitOfWork uow)
    { _albumRepo = albumRepo; _uow = uow; }

    public async Task<Guid> Handle(CreateAlbumCommand cmd, CancellationToken ct)
    {
        var album = Album.Create(cmd.Title, cmd.ArtistId, cmd.ReleaseYear);
        await _albumRepo.AddAsync(album, ct);
        await _uow.SaveChangesAsync(ct);
        return album.Id;
    }
}
