using FluentValidation;
using MusicApp.Domain.Interfaces;

namespace MusicApp.Application.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator(IUserRepository userRepo)
    {
        RuleFor(x => x.Email)
            .NotEmpty().EmailAddress()
            .MustAsync(async (email, ct) => !await userRepo.ExistsByEmailAsync(email, ct))
            .WithMessage("Email already in use.");

        RuleFor(x => x.Password)
            .NotEmpty().MinimumLength(8)
            .Matches(@"[A-Z]").WithMessage("Must contain uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Must contain lowercase letter.")
            .Matches(@"\d").WithMessage("Must contain a digit.")
            .Matches(@"[\W_]").WithMessage("Must contain a special character.");

    }
}
