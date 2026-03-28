namespace MusicApp.Domain.Entities;

public class PlaylistTrack
{
    public Guid PlaylistId { get; set; }
    public Guid TrackId { get; set; }
    public int Order { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    public Playlist Playlist { get; set; } = default!;
    public Track Track { get; set; } = default!;
}
