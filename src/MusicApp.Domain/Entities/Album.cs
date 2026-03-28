using MusicApp.Domain.Common;
using MusicApp.Domain.Enums;

namespace MusicApp.Domain.Entities;

public class Album : BaseEntity
{
    public string Title { get; private set; } = default!;
    public Guid ArtistId { get; private set; }
    public string? CoverImageUrl { get; private set; }
    public int? ReleaseYear { get; private set; }
    public AlbumStatus Status { get; private set; }

    public Artist Artist { get; private set; } = default!;

    private readonly List<Track> _tracks = new();
    public IReadOnlyList<Track> Tracks => _tracks.AsReadOnly();

    public static Album Create(string title, Guid artistId, int? releaseYear = null)
    {
        Guard.Against.NullOrWhiteSpace(title, nameof(title));
        return new Album
        {
            Title = title,
            ArtistId = artistId,
            ReleaseYear = releaseYear,
            Status = AlbumStatus.Draft
        };
    }

    public void Update(string? title, string? coverImageUrl, int? releaseYear)
    {
        if (title is not null) Title = title;
        if (coverImageUrl is not null) CoverImageUrl = coverImageUrl;
        if (releaseYear.HasValue) ReleaseYear = releaseYear.Value;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Publish()
    {
        Status = AlbumStatus.Published;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Remove()
    {
        Status = AlbumStatus.Removed;
        UpdatedAt = DateTime.UtcNow;
    }
}
