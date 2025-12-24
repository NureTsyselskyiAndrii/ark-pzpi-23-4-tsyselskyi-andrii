using FluentValidation;
using SafeDose.Application.Models.Identity.LogIn;

namespace SafeDose.Identity.Validators
{
    public class LogInValidator : AbstractValidator<LogInRequest>
    {
        public LogInValidator()
        {
            RuleFor(r => r.Login)
                   .Cascade(CascadeMode.Stop)
                   .NotEmpty().WithMessage("Username or Email is required")
                   .MinimumLength(3).WithMessage("Username is incorrect.")
                   .When(r => !r.Login.Contains('@'))
                   .MaximumLength(30).WithMessage("Username is incorrect.")
                   .When(r => !r.Login.Contains('@'))
                   .Matches(@"^[a-zA-Z0-9._]+$").WithMessage("Username is incorrect.")
                   .When(r => !r.Login.Contains('@'))
                   .Must(username => !username.Contains("..") && !username.Contains("__")).WithMessage("Username is incorrect.")
                   .When(r => !r.Login.Contains('@'))
                   .MaximumLength(511).WithMessage("The email address is invalid.")
                   .When(r => r.Login.Contains('@'))
                   .EmailAddress().WithMessage("The email address is invalid.")
                   .When(r => r.Login.Contains('@'))
                   .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").WithMessage("The email address is invalid.")
                   .When(r => r.Login.Contains('@'));

            RuleFor(r => r.Password)
                   .Cascade(CascadeMode.Stop)
                   .NotEmpty().WithMessage("Password is required.");

            RuleFor(r => r.DeviceId)
                   .Cascade(CascadeMode.Stop)
                   .NotEmpty().WithMessage("DeviceId is required.");
        }
    }
}
