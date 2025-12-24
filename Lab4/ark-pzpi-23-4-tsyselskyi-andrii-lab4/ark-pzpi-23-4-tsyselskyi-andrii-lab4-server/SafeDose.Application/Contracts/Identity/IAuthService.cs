using SafeDose.Application.Models.Identity.ForgotPassword;
using SafeDose.Application.Models.Identity.General;
using SafeDose.Application.Models.Identity.GoogleAuth;
using SafeDose.Application.Models.Identity.LogIn;
using SafeDose.Application.Models.Identity.RefreshToken;
using SafeDose.Application.Models.Identity.Registration;
using System.Security.Claims;

namespace SafeDose.Application.Contracts.Identity
{
    public interface IAuthService
    {
        Task<CompleteAuthResponse> GoogleLogin(GoogleAuthRequest request);
        Task<CompleteAuthResponse> Login(LogInRequest request);
        Task<RegistrationStep1Response> RegistrationStep1(RegistrationStep1Request request);
        Task RegistrationStep2(RegistrationStep2Request request, string? registrationToken);
        Task<ProfilePictureUploadResponse> UploadProfilePicture(ProfilePictureUploadRequest request, string? registrationToken);
        Task ResendConfirmationCode(string? registrationToken);
        Task RegistrationStep3(RegistrationStep3Request request, string? registrationToken);
        Task<CompleteAuthResponse> RefreshToken(RefreshTokenRequest request, string? refreshToken);
        Task ForgotPassword(ForgotPasswordRequest request);
        Task ResetPassword(ResetPasswordRequest request);
        Task Logout(ClaimsPrincipal userPrincipal);
    }
}
