using MediatR;

namespace MusicApp.Application.Tracks.Commands.RecordPlay;

public record RecordPlayCommand(Guid TrackId, Guid UserId) : IRequest<PlayResultDto>;
public record PlayResultDto(string StreamUrl);
