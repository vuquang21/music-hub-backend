namespace MusicApp.Application.Search.DTOs;

public record RecentSearchDto(
    Guid Id,
    Guid TrackId,
    string Title,
    string? ArtistName,
    string? CoverImageUrl,
    DateTime SearchedAt
);
