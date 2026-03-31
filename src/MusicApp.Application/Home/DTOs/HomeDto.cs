namespace MusicApp.Application.Home.DTOs;

/// <summary>Lightweight track card shown inside a home feed section.</summary>
public record HomeSectionTrackDto(
    Guid Id,
    string Title,
    string? ArtistName,
    string? CoverImageUrl,
    string? CdnUrl,
    int DurationSeconds,
    string DurationFormatted,
    int PlayCount
);

/// <summary>Podcast card shown in the Podcast section of the home feed.</summary>
public record HomePodcastDto(
    Guid Id,
    string Title,
    string? HostName,
    string? CoverImageUrl,
    int EpisodeCount
);

/// <summary>Aggregated payload for the home screen — all four sections in one response.</summary>
public record HomeDto(
    IReadOnlyList<HomeSectionTrackDto> Trending,
    IReadOnlyList<HomeSectionTrackDto> ForYou,
    IReadOnlyList<HomeSectionTrackDto> DiscoveryWeekly,
    IReadOnlyList<HomePodcastDto> Podcasts
);
