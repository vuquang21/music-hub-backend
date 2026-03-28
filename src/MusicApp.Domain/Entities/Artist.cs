using MusicApp.Domain.Common;

namespace MusicApp.Domain.Entities;

public class Artist : BaseEntity
{
    public string Name { get; private set; } = default!;
    public string? Bio { get; private set; }
    public string? ImageUrl { get; private set; }
    public Guid UserId { get; private set; }

    public User User { get; private set; } = default!;

    private readonly List<Track> _tracks = new();
    public IReadOnlyList<Track> Tracks => _tracks.AsReadOnly();

    private readonly List<Album> _albums = new();
    public IReadOnlyList<Album> Albums => _albums.AsReadOnly();

    public static Artist Create(string name, Guid userId)
    {
        Guard.Against.NullOrWhiteSpace(name, nameof(name));
        return new Artist { Name = name, UserId = userId };
    }

    public void UpdateProfile(string? name, string? bio, string? imageUrl)
    {
        if (name is not null) Name = name;
        if (bio is not null) Bio = bio;
        if (imageUrl is not null) ImageUrl = imageUrl;
        UpdatedAt = DateTime.UtcNow;
    }
}
