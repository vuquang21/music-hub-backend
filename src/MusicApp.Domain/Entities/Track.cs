using MusicApp.Domain.Common;
using MusicApp.Domain.Enums;
using MusicApp.Domain.Events;
using MusicApp.Domain.ValueObjects;

namespace MusicApp.Domain.Entities;

public class Track : BaseEntity
{
    public string Title { get; private set; } = default!;
    public Guid ArtistId { get; private set; }
    public Guid? AlbumId { get; private set; }
    public ISRC Isrc { get; private set; } = default!;
    public Duration Duration { get; private set; } = default!;
    public AudioQuality? Quality { get; private set; }
    public string? StorageKey { get; private set; }
    public string? CdnUrl { get; private set; }
    public int PlayCount { get; private set; }
    public TrackStatus Status { get; private set; }

    public Artist Artist { get; private set; } = default!;
    public Album? Album { get; private set; }

    private readonly List<Genre> _genres = new();
    public IReadOnlyList<Genre> Genres => _genres.AsReadOnly();

    public static Track Create(string title, Guid artistId, ISRC isrc, Duration duration)
    {
        Guard.Against.NullOrWhiteSpace(title, nameof(title));
        var track = new Track
        {
            Title = title,
            ArtistId = artistId,
            Isrc = isrc,
            Duration = duration,
            Status = TrackStatus.Draft
        };
        track.AddDomainEvent(new TrackCreatedEvent(track.Id));
        return track;
    }

    public void SetStorageKey(string storageKey) => StorageKey = storageKey;
    public void SetCdnUrl(string cdnUrl) => CdnUrl = cdnUrl;

    public void Update(string? title, Guid? albumId)
    {
        if (title is not null) Title = title;
        if (albumId.HasValue) AlbumId = albumId.Value;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Publish()
    {
        Status = TrackStatus.Published;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Remove()
    {
        Status = TrackStatus.Removed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void IncrementPlayCount()
    {
        PlayCount++;
        AddDomainEvent(new TrackPlayedEvent(Id, ArtistId));
    }

    public void AddGenre(Genre genre) => _genres.Add(genre);
}
