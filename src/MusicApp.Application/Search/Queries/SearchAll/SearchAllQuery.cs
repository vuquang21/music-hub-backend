using MediatR;
using MusicApp.Application.Search.DTOs;

namespace MusicApp.Application.Search.Queries.SearchAll;

public record SearchAllQuery(string Q, int Limit = 20) : IRequest<SearchResultDto>;
