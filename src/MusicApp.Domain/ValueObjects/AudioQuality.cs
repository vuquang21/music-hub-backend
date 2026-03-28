using MusicApp.Domain.Exceptions;

namespace MusicApp.Domain.ValueObjects;

public record AudioQuality
{
    public int BitRate { get; }
    public string Format { get; }

    public AudioQuality(int bitRate, string format)
    {
        if (bitRate <= 0) throw new DomainException("BitRate must be positive.");
        if (string.IsNullOrWhiteSpace(format)) throw new DomainException("Format is required.");
        BitRate = bitRate;
        Format = format;
    }
}
