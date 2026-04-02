using MediatR;
using MusicApp.Application.Search.DTOs;

namespace MusicApp.Application.Search.Queries.GetRecentSearches;

public record GetRecentSearchesQuery(Guid UserId) : IRequest<List<RecentSearchDto>>;
