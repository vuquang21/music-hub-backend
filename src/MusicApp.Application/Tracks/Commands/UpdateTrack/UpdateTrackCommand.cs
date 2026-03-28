using MediatR;

namespace MusicApp.Application.Tracks.Commands.UpdateTrack;

public record UpdateTrackCommand(Guid Id, string? Title, Guid? AlbumId) : IRequest;
