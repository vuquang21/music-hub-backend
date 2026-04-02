using MediatR;
using MusicApp.Application.Genres.DTOs;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Application.Genres.Queries.GetAllGenres;

public class GetAllGenresQueryHandler : IRequestHandler<GetAllGenresQuery, List<GenreDto>>
{
    private readonly IGenreRepository _genreRepo;

    public GetAllGenresQueryHandler(IGenreRepository genreRepo)
        => _genreRepo = genreRepo;

    public async Task<List<GenreDto>> Handle(GetAllGenresQuery request, CancellationToken ct)
    {
        var genres = await _genreRepo.GetAllAsync(ct);

        return genres.Select(g => new GenreDto(g.Id, g.Name, g.Slug)).ToList();
    }
}
