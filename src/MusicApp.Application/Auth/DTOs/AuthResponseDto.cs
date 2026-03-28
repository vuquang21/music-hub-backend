namespace MusicApp.Application.Auth.DTOs;

public record AuthResponseDto(
    string AccessToken,
    int ExpiresIn,
    string TokenType,
    UserDto User,
    string? RefreshToken = null);

public record UserDto(
    Guid Id,
    string Email,
    string DisplayName,
    string? AvatarUrl,
    string Role);
