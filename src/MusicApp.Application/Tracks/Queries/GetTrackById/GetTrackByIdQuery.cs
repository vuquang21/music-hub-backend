using MediatR;
using MusicApp.Application.Tracks.DTOs;

namespace MusicApp.Application.Tracks.Queries.GetTrackById;

public record GetTrackByIdQuery(Guid Id) : IRequest<TrackDto>;
