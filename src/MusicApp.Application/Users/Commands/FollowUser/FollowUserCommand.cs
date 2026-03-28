using MediatR;

namespace MusicApp.Application.Users.Commands.FollowUser;

public record FollowUserCommand(Guid FollowerId, Guid TargetUserId) : IRequest;
