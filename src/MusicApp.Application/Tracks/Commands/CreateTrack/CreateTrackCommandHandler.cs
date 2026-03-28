using MediatR;
using MusicApp.Application.Common.Interfaces;
using MusicApp.Domain.Entities;
using MusicApp.Domain.Interfaces;
using MusicApp.Domain.ValueObjects;

namespace MusicApp.Application.Tracks.Commands.CreateTrack;

public class CreateTrackCommandHandler : IRequestHandler<CreateTrackCommand, Guid>
{
    private readonly ITrackRepository _trackRepo;
    private readonly IGenreRepository _genreRepo;
    private readonly IStorageService _storage;
    private readonly IUnitOfWork _uow;

    public CreateTrackCommandHandler(
        ITrackRepository trackRepo, IGenreRepository genreRepo,
        IStorageService storage, IUnitOfWork uow)
    {
        _trackRepo = trackRepo;
        _genreRepo = genreRepo;
        _storage = storage;
        _uow = uow;
    }

    public async Task<Guid> Handle(CreateTrackCommand cmd, CancellationToken ct)
    {
        var isrc = new ISRC(cmd.Isrc);
        var duration = new Duration(cmd.DurationSeconds);
        var track = Track.Create(cmd.Title, cmd.ArtistId, isrc, duration);

        var storageKey = await _storage.UploadAudioAsync(cmd.AudioFile, ct);
        track.SetStorageKey(storageKey);

        if (cmd.Genres is { Count: > 0 })
        {
            var genres = await _genreRepo.GetBySlugsAsync(cmd.Genres, ct);
            foreach (var genre in genres) track.AddGenre(genre);
        }

        if (cmd.AlbumId.HasValue)
            track.Update(null, cmd.AlbumId);

        await _trackRepo.AddAsync(track, ct);
        await _uow.SaveChangesAsync(ct);
        return track.Id;
    }
}
