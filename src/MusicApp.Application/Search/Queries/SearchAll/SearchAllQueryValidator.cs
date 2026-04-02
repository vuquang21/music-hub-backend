using FluentValidation;

namespace MusicApp.Application.Search.Queries.SearchAll;

public class SearchAllQueryValidator : AbstractValidator<SearchAllQuery>
{
    public SearchAllQueryValidator()
    {
        RuleFor(x => x.Q)
            .NotEmpty().WithMessage("Search query is required.")
            .MinimumLength(1).WithMessage("Search query must be at least 1 character.");

        RuleFor(x => x.Limit)
            .InclusiveBetween(1, 50).WithMessage("Limit must be between 1 and 50.");
    }
}
