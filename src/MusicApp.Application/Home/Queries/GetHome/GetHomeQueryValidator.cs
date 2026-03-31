using FluentValidation;

namespace MusicApp.Application.Home.Queries.GetHome;

public class GetHomeQueryValidator : AbstractValidator<GetHomeQuery>
{
    public GetHomeQueryValidator()
    {
        RuleFor(x => x.SectionLimit)
            .InclusiveBetween(1, 20)
            .WithMessage("SectionLimit must be between 1 and 20.");
    }
}
