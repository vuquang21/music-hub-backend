namespace MusicApp.Application.Tracks.DTOs;

public record TrackDto(
    Guid Id, string Title, Guid ArtistId, string? ArtistName, Guid? AlbumId,
    string Isrc, int DurationSeconds, string DurationFormatted,
    string? CdnUrl, int PlayCount, string Status, List<string> Genres, DateTime CreatedAt);
