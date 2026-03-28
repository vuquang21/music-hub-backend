using MediatR;
using MusicApp.Application.Common.DTOs;
using MusicApp.Application.Tracks.DTOs;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Application.Tracks.Queries.GetTracksPaged;

public class GetTracksPagedQuery : PagedQueryParams, IRequest<PagedResult<TrackDto>>
{
    public string? Genre { get; set; }
    public Guid? ArtistId { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}
