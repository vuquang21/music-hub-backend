using MediatR;
using MusicApp.Domain.Exceptions;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Application.Albums.Commands.PublishAlbum;

public class PublishAlbumCommandHandler : IRequestHandler<PublishAlbumCommand>
{
    private readonly IAlbumRepository _albumRepo;
    private readonly IUnitOfWork _uow;

    public PublishAlbumCommandHandler(IAlbumRepository albumRepo, IUnitOfWork uow)
    { _albumRepo = albumRepo; _uow = uow; }

    public async Task Handle(PublishAlbumCommand cmd, CancellationToken ct)
    {
        var album = await _albumRepo.GetByIdAsync(cmd.Id, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Album), cmd.Id);
        album.Publish();
        await _uow.SaveChangesAsync(ct);
    }
}
