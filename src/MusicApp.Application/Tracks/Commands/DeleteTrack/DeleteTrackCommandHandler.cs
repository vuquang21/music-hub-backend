using MediatR;
using MusicApp.Domain.Exceptions;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Application.Tracks.Commands.DeleteTrack;

public class DeleteTrackCommandHandler : IRequestHandler<DeleteTrackCommand>
{
    private readonly ITrackRepository _trackRepo;
    private readonly IUnitOfWork _uow;

    public DeleteTrackCommandHandler(ITrackRepository trackRepo, IUnitOfWork uow)
    { _trackRepo = trackRepo; _uow = uow; }

    public async Task Handle(DeleteTrackCommand cmd, CancellationToken ct)
    {
        var track = await _trackRepo.GetByIdAsync(cmd.Id, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Track), cmd.Id);
        track.Remove();
        await _uow.SaveChangesAsync(ct);
    }
}
