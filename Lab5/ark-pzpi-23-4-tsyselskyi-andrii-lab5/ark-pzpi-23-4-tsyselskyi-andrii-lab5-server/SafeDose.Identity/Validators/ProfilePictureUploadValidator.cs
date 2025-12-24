using FluentValidation;
using Microsoft.AspNetCore.Http;
using SafeDose.Application.Models.Identity.Registration;
using SixLabors.ImageSharp;

namespace SafeDose.Identity.Validators
{
    public class ProfilePictureUploadValidator : AbstractValidator<ProfilePictureUploadRequest>
    {
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private static readonly string[] AllowedContentTypes = { "image/jpeg", "image/png", "image/webp" };
        private const long MaxFileSizeInBytes = 5 * 1024 * 1024;
        public ProfilePictureUploadValidator()
        {
            RuleFor(x => x.File)
                   .Cascade(CascadeMode.Stop)
                   .NotNull()
                   .WithMessage("The picture is required.")
                   .Must(file => file != null && file.Length > 0)
                   .WithMessage("The picture is empty.")
                   .Must(file => file.Length <= MaxFileSizeInBytes)
                   .WithMessage($"The picture must be less than {MaxFileSizeInBytes / 1024 / 1024} MB.")
                   .Must(file => AllowedContentTypes.Contains(file.ContentType))
                   .WithMessage("Only JPG, PNG, and WEBP formats are allowed.")
                   .Must(file =>
                   {
                       var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                       return AllowedExtensions.Contains(extension);
                   })
                   .WithMessage("File extension must be .jpg, .jpeg, .png or .webp.")
                   .Must(BeValidImage)
                   .WithMessage("The file is not a valid image.");
        }

        private bool BeValidImage(IFormFile file)
        {
            try
            {
                using var image = Image.Load(file.OpenReadStream());
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
