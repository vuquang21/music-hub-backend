using MediatR;
using MusicApp.Domain.Exceptions;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Application.Albums.Commands.UpdateAlbum;

public class UpdateAlbumCommandHandler : IRequestHandler<UpdateAlbumCommand>
{
    private readonly IAlbumRepository _albumRepo;
    private readonly IUnitOfWork _uow;

    public UpdateAlbumCommandHandler(IAlbumRepository albumRepo, IUnitOfWork uow)
    { _albumRepo = albumRepo; _uow = uow; }

    public async Task Handle(UpdateAlbumCommand cmd, CancellationToken ct)
    {
        var album = await _albumRepo.GetByIdAsync(cmd.Id, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Album), cmd.Id);
        album.Update(cmd.Title, cmd.CoverImageUrl, cmd.ReleaseYear);
        await _uow.SaveChangesAsync(ct);
    }
}
