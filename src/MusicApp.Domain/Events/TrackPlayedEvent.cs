using MusicApp.Domain.Common;

namespace MusicApp.Domain.Events;

public record TrackPlayedEvent(Guid TrackId, Guid ArtistId) : IDomainEvent;
