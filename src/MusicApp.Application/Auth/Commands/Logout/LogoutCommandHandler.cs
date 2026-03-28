using MediatR;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Application.Auth.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly IUserRepository _userRepo;
    private readonly IUnitOfWork _uow;

    public LogoutCommandHandler(IUserRepository userRepo, IUnitOfWork uow)
    {
        _userRepo = userRepo;
        _uow = uow;
    }

    public async Task Handle(LogoutCommand cmd, CancellationToken ct)
    {
        var user = await _userRepo.GetByRefreshTokenAsync(cmd.RefreshToken, ct);
        if (user is null) return;
        user.RevokeRefreshToken(cmd.RefreshToken, "Logged out");
        await _uow.SaveChangesAsync(ct);
    }
}
