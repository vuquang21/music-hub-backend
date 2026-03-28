using MediatR;

namespace MusicApp.Application.Users.Commands.LikeTrack;

public record LikeTrackCommand(Guid UserId, Guid TrackId) : IRequest;
