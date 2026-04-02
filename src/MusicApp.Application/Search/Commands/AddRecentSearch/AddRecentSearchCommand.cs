using MediatR;

namespace MusicApp.Application.Search.Commands.AddRecentSearch;

public record AddRecentSearchCommand(Guid UserId, Guid TrackId) : IRequest<Unit>;
