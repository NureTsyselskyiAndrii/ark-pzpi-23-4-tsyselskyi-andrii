using Google.Apis.Auth;
using SafeDose.Application.Models.Identity.GoogleAuth;

namespace SafeDose.Application.Contracts.Identity
{
    public interface IGoogleAuthService
    {
        Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(GoogleAuthRequest authRequest);
    }
}
