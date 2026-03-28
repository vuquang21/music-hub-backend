using MediatR;

namespace MusicApp.Application.Tracks.Commands.DeleteTrack;

public record DeleteTrackCommand(Guid Id) : IRequest;
