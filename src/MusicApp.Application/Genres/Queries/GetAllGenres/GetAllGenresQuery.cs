using MediatR;
using MusicApp.Application.Genres.DTOs;

namespace MusicApp.Application.Genres.Queries.GetAllGenres;

public record GetAllGenresQuery : IRequest<List<GenreDto>>;
