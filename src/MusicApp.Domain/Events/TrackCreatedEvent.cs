using MusicApp.Domain.Common;

namespace MusicApp.Domain.Events;

public record TrackCreatedEvent(Guid TrackId) : IDomainEvent;
