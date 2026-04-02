using MediatR;

namespace MusicApp.Application.Search.Commands.DeleteRecentSearch;

public record DeleteRecentSearchCommand(Guid UserId, Guid SearchHistoryId) : IRequest<Unit>;
