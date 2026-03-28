using MediatR;
using MusicApp.Domain.Exceptions;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Application.Albums.Commands.DeleteAlbum;

public class DeleteAlbumCommandHandler : IRequestHandler<DeleteAlbumCommand>
{
    private readonly IAlbumRepository _albumRepo;
    private readonly IUnitOfWork _uow;

    public DeleteAlbumCommandHandler(IAlbumRepository albumRepo, IUnitOfWork uow)
    { _albumRepo = albumRepo; _uow = uow; }

    public async Task Handle(DeleteAlbumCommand cmd, CancellationToken ct)
    {
        var album = await _albumRepo.GetByIdAsync(cmd.Id, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Album), cmd.Id);
        album.Remove();
        await _uow.SaveChangesAsync(ct);
    }
}
