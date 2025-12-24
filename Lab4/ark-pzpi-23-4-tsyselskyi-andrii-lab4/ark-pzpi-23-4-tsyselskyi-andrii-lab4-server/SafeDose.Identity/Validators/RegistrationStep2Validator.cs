using FluentValidation;
using SafeDose.Application.Models.Identity.Registration;

namespace SafeDose.Identity.Validators
{
    public class RegistrationStep2Validator : AbstractValidator<RegistrationStep2Request>
    {
        public RegistrationStep2Validator()
        {
            RuleFor(r => r.FirstName)
                   .Cascade(CascadeMode.Stop)
                   .NotEmpty().WithMessage("First name is required")
                   .MinimumLength(3).WithMessage("First name must be at least 2 characters long")
                   .MaximumLength(20).WithMessage("First name must be at most 20 characters long")
                   .Matches(@"^[^\d\W_]+(?:[' ][^\d\W_]+)*$").WithMessage("First name can only contain letters, spaces, and apostrophes");

            RuleFor(r => r.LastName)
                   .Cascade(CascadeMode.Stop)
                   .NotEmpty().WithMessage("Last name is required")
                   .MinimumLength(3).WithMessage("Last name must be at least 2 characters long")
                   .MaximumLength(20).WithMessage("Last name must be at most 20 characters long")
                   .Matches(@"^[^\d\W_]+(?:[' ][^\d\W_]+)*$").WithMessage("Last name can only contain letters, spaces, and apostrophes");

            RuleFor(r => r.BirthDate)
                   .Cascade(CascadeMode.Stop)
                   .NotEmpty().WithMessage("Birth date name is required")
                   .Matches(@"^\d{2}/\d{2}/\d{4}$").WithMessage("The date format must be MM/dd/yyyy")
                   .Must(BeAValidDate).WithMessage(r => GetDateErrorMessage(r.BirthDate));
        }

        private bool BeAValidDate(string date)
        {
            if (!DateTime.TryParseExact(date, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
            {
                return false;
            }

            int currentYear = DateTime.UtcNow.Year;
            return parsedDate.Year <= currentYear &&
                   parsedDate.Year + 13 <= currentYear &&
                   parsedDate.Year + 140 >= currentYear;
        }

        private string GetDateErrorMessage(string date)
        {
            if (!DateTime.TryParseExact(date, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
            {
                return "The date is not valid.";
            }

            int currentYear = DateTime.UtcNow.Year;
            if (parsedDate.Year > currentYear)
            {
                return "Are you from the future?";
            }
            if (parsedDate.Year + 13 > currentYear)
            {
                return "You are too young.";
            }
            if (parsedDate.Year + 140 < currentYear)
            {
                return "You are too old.";
            }

            return "Invalid birth date.";
        }
    }
}
