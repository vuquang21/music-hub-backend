using MusicApp.Domain.Enums;

namespace MusicApp.Application.Common.Interfaces;

public interface ICurrentUser
{
    Guid Id { get; }
    string Email { get; }
    UserRole Role { get; }
    bool IsAuthenticated { get; }
}
