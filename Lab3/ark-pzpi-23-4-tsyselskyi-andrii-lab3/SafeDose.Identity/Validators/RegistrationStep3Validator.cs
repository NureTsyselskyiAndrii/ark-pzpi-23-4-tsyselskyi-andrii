using FluentValidation;
using SafeDose.Application.Models.Identity.Registration;

namespace SafeDose.Identity.Validators
{
    public class RegistrationStep3Validator : AbstractValidator<RegistrationStep3Request>
    {
        public RegistrationStep3Validator()
        {
            RuleFor(r => r.Code)
                   .Cascade(CascadeMode.Stop)
                   .NotEmpty().WithMessage("Confirmation code is required")
                   .Length(6).WithMessage("The code is not complete.")
                   .Matches(@"^[0-9]+$").WithMessage("The code must only contain numbers.");
        }
    }
}
