using MediatR;
using MusicApp.Domain.Exceptions;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Application.Tracks.Commands.UpdateTrack;

public class UpdateTrackCommandHandler : IRequestHandler<UpdateTrackCommand>
{
    private readonly ITrackRepository _trackRepo;
    private readonly IUnitOfWork _uow;

    public UpdateTrackCommandHandler(ITrackRepository trackRepo, IUnitOfWork uow)
    { _trackRepo = trackRepo; _uow = uow; }

    public async Task Handle(UpdateTrackCommand cmd, CancellationToken ct)
    {
        var track = await _trackRepo.GetByIdAsync(cmd.Id, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Track), cmd.Id);
        track.Update(cmd.Title, cmd.AlbumId);
        await _uow.SaveChangesAsync(ct);
    }
}
