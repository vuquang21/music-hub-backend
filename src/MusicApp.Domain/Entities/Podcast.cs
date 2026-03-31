using MusicApp.Domain.Common;

namespace MusicApp.Domain.Entities;

/// <summary>Represents a podcast show with one or more episodes.</summary>
public class Podcast : BaseEntity
{
    public string Title { get; private set; } = default!;
    public string? Description { get; private set; }
    public string? CoverImageUrl { get; private set; }
    public string? HostName { get; private set; }
    public int EpisodeCount { get; private set; }
    public bool IsActive { get; private set; }

    private Podcast() { }

    /// <summary>Creates a new active podcast.</summary>
    public static Podcast Create(string title, string? hostName, string? description = null)
    {
        Guard.Against.NullOrWhiteSpace(title, nameof(title));
        return new Podcast
        {
            Title = title.Trim(),
            HostName = hostName?.Trim(),
            Description = description?.Trim(),
            EpisodeCount = 0,
            IsActive = true
        };
    }

    public void Update(string? title, string? description, string? coverImageUrl, string? hostName)
    {
        if (title is not null) Title = title.Trim();
        if (description is not null) Description = description.Trim();
        if (coverImageUrl is not null) CoverImageUrl = coverImageUrl;
        if (hostName is not null) HostName = hostName.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetCoverImage(string coverImageUrl)
    {
        CoverImageUrl = coverImageUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public void IncrementEpisodeCount()
    {
        EpisodeCount++;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
