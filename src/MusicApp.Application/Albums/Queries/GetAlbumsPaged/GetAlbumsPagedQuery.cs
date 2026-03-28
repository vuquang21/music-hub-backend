using MediatR;
using MusicApp.Application.Albums.DTOs;
using MusicApp.Application.Common.DTOs;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Application.Albums.Queries.GetAlbumsPaged;

public class GetAlbumsPagedQuery : PagedQueryParams, IRequest<PagedResult<AlbumDto>>
{
    public Guid? ArtistId { get; set; }
}
