using MediatR;
using Microsoft.AspNetCore.Http;

namespace MusicApp.Application.Tracks.Commands.CreateTrack;

public record CreateTrackCommand(
    string Title, Guid ArtistId, string Isrc, int DurationSeconds,
    IFormFile AudioFile, Guid? AlbumId = null, List<string>? Genres = null) : IRequest<Guid>;
