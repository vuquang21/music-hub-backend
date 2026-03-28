using MediatR;
using MusicApp.Domain.Exceptions;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Application.Users.Commands.LikeTrack;

public class LikeTrackCommandHandler : IRequestHandler<LikeTrackCommand>
{
    private readonly IUserRepository _userRepo;
    private readonly ITrackRepository _trackRepo;
    private readonly IUnitOfWork _uow;

    public LikeTrackCommandHandler(IUserRepository userRepo, ITrackRepository trackRepo, IUnitOfWork uow)
    { _userRepo = userRepo; _trackRepo = trackRepo; _uow = uow; }

    public async Task Handle(LikeTrackCommand cmd, CancellationToken ct)
    {
        var user = await _userRepo.GetByIdAsync(cmd.UserId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.User), cmd.UserId);
        var track = await _trackRepo.GetByIdAsync(cmd.TrackId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Track), cmd.TrackId);
        user.LikeTrack(track);
        await _uow.SaveChangesAsync(ct);
    }
}
