using MediatR;
using MusicApp.Application.Common.Interfaces;
using MusicApp.Domain.Exceptions;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Application.Tracks.Commands.RecordPlay;

public class RecordPlayCommandHandler : IRequestHandler<RecordPlayCommand, PlayResultDto>
{
    private readonly ITrackRepository _trackRepo;
    private readonly IStorageService _storage;
    private readonly IUnitOfWork _uow;

    public RecordPlayCommandHandler(ITrackRepository trackRepo, IStorageService storage, IUnitOfWork uow)
    { _trackRepo = trackRepo; _storage = storage; _uow = uow; }

    public async Task<PlayResultDto> Handle(RecordPlayCommand cmd, CancellationToken ct)
    {
        var track = await _trackRepo.GetByIdAsync(cmd.TrackId, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Track), cmd.TrackId);
        track.IncrementPlayCount();
        await _uow.SaveChangesAsync(ct);

        var url = track.StorageKey is not null
            ? _storage.GetPresignedUrl(track.StorageKey) : track.CdnUrl ?? "";
        return new PlayResultDto(url);
    }
}
