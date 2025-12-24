using Microsoft.AspNetCore.Http;

namespace SafeDose.Application.Models.Identity.Registration
{
    public class ProfilePictureUploadRequest
    {
        public IFormFile File { get; set; } = null!;
    }
}
