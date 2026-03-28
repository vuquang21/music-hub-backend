using FluentValidation;

namespace MusicApp.Application.Tracks.Commands.CreateTrack;

public class CreateTrackCommandValidator : AbstractValidator<CreateTrackCommand>
{
    public CreateTrackCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ArtistId).NotEmpty();
        RuleFor(x => x.Isrc).NotEmpty().Matches(@"^[A-Z]{2}-[A-Z0-9]{3}-\d{2}-\d{5}$");
        RuleFor(x => x.DurationSeconds).GreaterThan(0);
        RuleFor(x => x.AudioFile).NotNull();
    }
}
