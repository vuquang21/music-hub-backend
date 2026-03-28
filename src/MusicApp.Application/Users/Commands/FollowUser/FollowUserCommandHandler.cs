using MediatR;
using MusicApp.Domain.Exceptions;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Application.Users.Commands.FollowUser;

public class FollowUserCommandHandler : IRequestHandler<FollowUserCommand>
{
    private readonly IUserRepository _userRepo;
    private readonly IUnitOfWork _uow;

    public FollowUserCommandHandler(IUserRepository userRepo, IUnitOfWork uow)
    { _userRepo = userRepo; _uow = uow; }

    public async Task Handle(FollowUserCommand cmd, CancellationToken ct)
    {
        var follower = await _userRepo.GetByIdAsync(cmd.FollowerId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.User), cmd.FollowerId);
        var target = await _userRepo.GetByIdAsync(cmd.TargetUserId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.User), cmd.TargetUserId);
        follower.Follow(target);
        await _uow.SaveChangesAsync(ct);
    }
}
