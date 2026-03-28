namespace MusicApp.Application.Playlists.DTOs;

public record PlaylistDto(
    Guid Id, string Name, string? Description, string? CoverImageUrl,
    Guid OwnerId, string? OwnerName, bool IsPublic, int TrackCount,
    int FollowerCount, DateTime CreatedAt);
