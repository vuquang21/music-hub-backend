using MediatR;

namespace MusicApp.Application.Auth.Commands.Logout;

public record LogoutCommand(string RefreshToken) : IRequest;
