using MediatR;

namespace MusicApp.Application.Tracks.Commands.PublishTrack;

public record PublishTrackCommand(Guid Id) : IRequest;
