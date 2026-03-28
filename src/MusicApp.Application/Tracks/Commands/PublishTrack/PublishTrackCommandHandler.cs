using MediatR;
using MusicApp.Domain.Exceptions;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Application.Tracks.Commands.PublishTrack;

public class PublishTrackCommandHandler : IRequestHandler<PublishTrackCommand>
{
    private readonly ITrackRepository _trackRepo;
    private readonly IUnitOfWork _uow;

    public PublishTrackCommandHandler(ITrackRepository trackRepo, IUnitOfWork uow)
    { _trackRepo = trackRepo; _uow = uow; }

    public async Task Handle(PublishTrackCommand cmd, CancellationToken ct)
    {
        var track = await _trackRepo.GetByIdAsync(cmd.Id, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Track), cmd.Id);
        track.Publish();
        await _uow.SaveChangesAsync(ct);
    }
}
