using System.Text.RegularExpressions;
using MusicApp.Domain.Exceptions;

namespace MusicApp.Domain.ValueObjects;

public record ISRC
{
    public string Value { get; }
    private static readonly Regex Pattern =
        new(@"^[A-Z]{2}-[A-Z0-9]{3}-\d{2}-\d{5}$", RegexOptions.Compiled);

    public ISRC(string value)
    {
        if (!Pattern.IsMatch(value))
            throw new DomainException($"Invalid ISRC: {value}");
        Value = value.ToUpper();
    }

    public static implicit operator string(ISRC isrc) => isrc.Value;
}
