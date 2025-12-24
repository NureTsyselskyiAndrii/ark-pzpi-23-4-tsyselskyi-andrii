using FluentValidation;
using SafeDose.Application.Models.Identity.ForgotPassword;

namespace SafeDose.Identity.Validators
{
    public class ForgotPasswordVaidator : AbstractValidator<ForgotPasswordRequest>
    {
        public ForgotPasswordVaidator()
        {
            RuleFor(r => r.Email)
                  .Cascade(CascadeMode.Stop)
                  .NotEmpty().WithMessage("Email is required")
                  .MaximumLength(511).WithMessage("The email address is invalid.")
                  .EmailAddress().WithMessage("The email address is invalid.")
                  .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").WithMessage("The email address is invalid.");

            RuleFor(r => r.ClientUri)
                   .Cascade(CascadeMode.Stop)
                   .NotEmpty().WithMessage("ClientUri is required.")
                   .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out var temp) && (temp.Scheme == Uri.UriSchemeHttp || temp.Scheme == Uri.UriSchemeHttps))
                   .WithMessage("ClientUri must be a valid URL.");
        }
    }
}
