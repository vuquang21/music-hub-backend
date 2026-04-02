using MusicApp.Domain.Common;

namespace MusicApp.Domain.Entities;

public class SearchHistory : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid TrackId { get; private set; }
    public DateTime SearchedAt { get; private set; } = DateTime.UtcNow;

    public User User { get; private set; } = default!;
    public Track Track { get; private set; } = default!;

    public static SearchHistory Create(Guid userId, Guid trackId)
        => new() { UserId = userId, TrackId = trackId };

    public void Touch() => SearchedAt = DateTime.UtcNow;
}
