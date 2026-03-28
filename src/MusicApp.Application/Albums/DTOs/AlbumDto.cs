namespace MusicApp.Application.Albums.DTOs;

public record AlbumDto(
    Guid Id, string Title, Guid ArtistId, string? ArtistName,
    string? CoverImageUrl, int? ReleaseYear, string Status, int TrackCount, DateTime CreatedAt);
