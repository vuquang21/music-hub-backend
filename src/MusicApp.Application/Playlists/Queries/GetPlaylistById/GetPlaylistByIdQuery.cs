using MediatR;
using MusicApp.Application.Playlists.DTOs;

namespace MusicApp.Application.Playlists.Queries.GetPlaylistById;

public record GetPlaylistByIdQuery(Guid Id) : IRequest<PlaylistDto>;
