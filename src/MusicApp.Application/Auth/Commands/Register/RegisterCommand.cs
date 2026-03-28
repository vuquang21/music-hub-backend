using MediatR;
using MusicApp.Application.Auth.DTOs;

namespace MusicApp.Application.Auth.Commands.Register;

public record RegisterCommand(string Email, string Password, string DisplayName) : IRequest<AuthResponseDto>;
