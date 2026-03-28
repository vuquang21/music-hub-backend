namespace MusicApp.Application.Users.DTOs;

public record UserProfileDto(Guid Id, string DisplayName, string? AvatarUrl, string Role, DateTime CreatedAt);

public record ArtistDto(Guid Id, string Name, string? Bio, string? ImageUrl);
