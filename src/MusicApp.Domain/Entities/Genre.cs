using MusicApp.Domain.Common;

namespace MusicApp.Domain.Entities;

public class Genre : BaseEntity
{
    public string Name { get; private set; } = default!;
    public string Slug { get; private set; } = default!;

    private readonly List<Track> _tracks = new();
    public IReadOnlyList<Track> Tracks => _tracks.AsReadOnly();

    public static Genre Create(string name, string slug)
    {
        return new Genre { Name = name, Slug = slug };
    }
}
