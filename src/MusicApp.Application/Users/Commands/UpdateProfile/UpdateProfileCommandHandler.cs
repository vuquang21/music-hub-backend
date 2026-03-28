using MediatR;
using MusicApp.Domain.Exceptions;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Application.Users.Commands.UpdateProfile;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand>
{
    private readonly IUserRepository _userRepo;
    private readonly IUnitOfWork _uow;

    public UpdateProfileCommandHandler(IUserRepository userRepo, IUnitOfWork uow)
    { _userRepo = userRepo; _uow = uow; }

    public async Task Handle(UpdateProfileCommand cmd, CancellationToken ct)
    {
        var user = await _userRepo.GetByIdAsync(cmd.UserId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.User), cmd.UserId);
        user.UpdateProfile(cmd.DisplayName, cmd.AvatarUrl);
        await _uow.SaveChangesAsync(ct);
    }
}
