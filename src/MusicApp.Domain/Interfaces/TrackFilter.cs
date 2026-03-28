namespace MusicApp.Domain.Interfaces;

public class TrackFilter
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Search { get; set; }
    public string? Genre { get; set; }
    public Guid? ArtistId { get; set; }
    public string SortBy { get; set; } = "playCount";
    public string SortDir { get; set; } = "desc";
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}
