using MusicApp.Domain.Exceptions;

namespace MusicApp.Domain.ValueObjects;

public record Duration
{
    public int Seconds { get; }
    public Duration(int seconds)
    {
        if (seconds <= 0) throw new DomainException("Duration must be positive.");
        Seconds = seconds;
    }
    public string Formatted => TimeSpan.FromSeconds(Seconds).ToString(@"m\:ss");
}
