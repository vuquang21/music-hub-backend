using MediatR;
using MusicApp.Application.Auth.DTOs;

namespace MusicApp.Application.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string Token) : IRequest<AuthResponseDto>;
