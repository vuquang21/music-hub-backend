namespace MusicApp.Application.Search.DTOs;

public record SearchResultDto(IReadOnlyList<SearchResultItemDto> Items);

public record SearchResultItemDto(
    Guid Id,
    string Type,
    string Title,
    string? Subtitle,
    string? ImageUrl
);
