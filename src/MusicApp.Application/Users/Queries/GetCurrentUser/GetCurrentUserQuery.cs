using MediatR;
using MusicApp.Application.Auth.DTOs;

namespace MusicApp.Application.Users.Queries.GetCurrentUser;

public record GetCurrentUserQuery(Guid UserId) : IRequest<UserDto>;
