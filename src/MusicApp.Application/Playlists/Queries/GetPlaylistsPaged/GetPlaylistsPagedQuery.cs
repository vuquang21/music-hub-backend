using MediatR;
using MusicApp.Application.Common.DTOs;
using MusicApp.Application.Playlists.DTOs;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Application.Playlists.Queries.GetPlaylistsPaged;

public class GetPlaylistsPagedQuery : PagedQueryParams, IRequest<PagedResult<PlaylistDto>>
{
    public Guid? OwnerId { get; set; }
    public bool? IsPublic { get; set; }
}
