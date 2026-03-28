using MusicApp.Domain.Common;
using MusicApp.Domain.Exceptions;

namespace MusicApp.Domain.Entities;

public class Playlist : BaseEntity
{
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public string? CoverImageUrl { get; private set; }
    public Guid OwnerId { get; private set; }
    public bool IsPublic { get; private set; }

    public User Owner { get; private set; } = default!;

    private readonly List<PlaylistTrack> _playlistTracks = new();
    public IReadOnlyList<PlaylistTrack> PlaylistTracks => _playlistTracks.AsReadOnly();

    private readonly List<User> _followers = new();
    public IReadOnlyList<User> Followers => _followers.AsReadOnly();

    public static Playlist Create(string name, Guid ownerId, string? description = null, bool isPublic = true)
    {
        Guard.Against.NullOrWhiteSpace(name, nameof(name));
        return new Playlist
        {
            Name = name,
            OwnerId = ownerId,
            Description = description,
            IsPublic = isPublic
        };
    }

    public void Update(string? name, string? description, bool? isPublic, string? coverImageUrl)
    {
        if (name is not null) Name = name;
        if (description is not null) Description = description;
        if (isPublic.HasValue) IsPublic = isPublic.Value;
        if (coverImageUrl is not null) CoverImageUrl = coverImageUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddTrack(Guid trackId)
    {
        if (_playlistTracks.Any(pt => pt.TrackId == trackId))
            throw new DomainException("Track already in playlist.");
        var maxOrder = _playlistTracks.Count > 0 ? _playlistTracks.Max(pt => pt.Order) : 0;
        _playlistTracks.Add(new PlaylistTrack
        {
            PlaylistId = Id,
            TrackId = trackId,
            Order = maxOrder + 1
        });
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveTrack(Guid trackId)
    {
        var item = _playlistTracks.FirstOrDefault(pt => pt.TrackId == trackId)
            ?? throw new DomainException("Track not found in playlist.");
        _playlistTracks.Remove(item);
        UpdatedAt = DateTime.UtcNow;
    }

    public void ReorderTracks(List<Guid> trackIds)
    {
        for (int i = 0; i < trackIds.Count; i++)
        {
            var pt = _playlistTracks.FirstOrDefault(p => p.TrackId == trackIds[i]);
            if (pt is not null) pt.Order = i + 1;
        }
        UpdatedAt = DateTime.UtcNow;
    }
}
