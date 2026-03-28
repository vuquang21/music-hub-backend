using MusicApp.Domain.Common;

namespace MusicApp.Domain.Entities;

public class ListeningHistory : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid TrackId { get; private set; }
    public DateTime PlayedAt { get; private set; } = DateTime.UtcNow;

    public User User { get; private set; } = default!;
    public Track Track { get; private set; } = default!;

    public static ListeningHistory Create(Guid userId, Guid trackId)
        => new() { UserId = userId, TrackId = trackId };
}
