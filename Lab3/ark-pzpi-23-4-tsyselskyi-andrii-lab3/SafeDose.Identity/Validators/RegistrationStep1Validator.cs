using FluentValidation;
using SafeDose.Application.Models.Identity.Registration;

namespace SafeDose.Identity.Validators
{
    public class RegistrationStep1Validator : AbstractValidator<RegistrationStep1Request>
    {
        public RegistrationStep1Validator()
        {
            RuleFor(r => r.Email)
                   .Cascade(CascadeMode.Stop)
                   .NotEmpty().WithMessage("Email is required")
                   .MaximumLength(511).WithMessage("Email must be at most 511 characters long")
                   .EmailAddress().WithMessage("The email address is invalid.")
                   .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").WithMessage("The email address is invalid.");

            RuleFor(r => r.UserName)
                   .Cascade(CascadeMode.Stop)
                   .NotEmpty().WithMessage("Username is required")
                   .MinimumLength(3).WithMessage("Username must be at least 3 characters long")
                   .MaximumLength(30).WithMessage("Username must be at most 30 characters long")
                   .Matches(@"^[a-zA-Z0-9._]+$").WithMessage("Username can only contain letters, numbers, dots, and underscores")
                   .Must(username => !username.Contains("..") && !username.Contains("__"))
                   .WithMessage("Username cannot contain consecutive dots or underscores");

            RuleFor(r => r.Password)
                   .Cascade(CascadeMode.Stop)
                   .NotEmpty().WithMessage("Password is required.")
                   .MinimumLength(12).WithMessage("Password must be at least 12 characters long.")
                   .MaximumLength(40).WithMessage("Password must not exceed 40 characters.")
                   .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                   .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                   .Matches(@"[0-9]").WithMessage("Password must contain at least one number.")
                   .Matches(@"^[^\s]*$").WithMessage("Password cannot contain spaces.")
                   .Must(password => password.All(c => char.IsLetterOrDigit(c))).WithMessage("Password cannot contain symbols.");

            RuleFor(r => r.ConfirmPassword)
                   .Cascade(CascadeMode.Stop)
                   .NotEmpty().WithMessage("Re-password is required.")
                   .Equal(r => r.Password).WithMessage("Passwords do not match.");
        }
    }
}
