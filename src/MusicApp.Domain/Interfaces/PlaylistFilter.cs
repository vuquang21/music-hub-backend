namespace MusicApp.Domain.Interfaces;

public class PlaylistFilter
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Search { get; set; }
    public Guid? OwnerId { get; set; }
    public bool? IsPublic { get; set; }
    public string SortBy { get; set; } = "createdAt";
    public string SortDir { get; set; } = "desc";
}
