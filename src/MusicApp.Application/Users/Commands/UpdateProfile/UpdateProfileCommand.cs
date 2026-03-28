using MediatR;

namespace MusicApp.Application.Users.Commands.UpdateProfile;

public record UpdateProfileCommand(Guid UserId, string? DisplayName, string? AvatarUrl) : IRequest;
