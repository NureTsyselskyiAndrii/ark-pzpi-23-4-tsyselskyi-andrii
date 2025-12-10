using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeDose.Application.Contracts.Identity;
using SafeDose.Application.Models.Identity.ForgotPassword;
using SafeDose.Application.Models.Identity.General;
using SafeDose.Application.Models.Identity.GoogleAuth;
using SafeDose.Application.Models.Identity.LogIn;
using SafeDose.Application.Models.Identity.RefreshToken;
using SafeDose.Application.Models.Identity.Registration;

namespace SafeDose.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("google-login")]
        [ProducesResponseType(typeof(CompleteAuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GoogleLogin(GoogleAuthRequest googleRequest)
        {
            var completeAuthResponse = await _authService.GoogleLogin(googleRequest);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = completeAuthResponse.RefreshTokenResponse.ExpiryTime
            };
            Response.Cookies.Append("refreshToken", completeAuthResponse.RefreshTokenResponse.Token, cookieOptions);

            return Ok(completeAuthResponse.AuthResponse);
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(CompleteAuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login(LogInRequest logInRequest)
        {
            var completeAuthResponse = await _authService.Login(logInRequest);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = completeAuthResponse.RefreshTokenResponse.ExpiryTime
            };
            Response.Cookies.Append("refreshToken", completeAuthResponse.RefreshTokenResponse.Token, cookieOptions);

            return Ok(completeAuthResponse.AuthResponse);
        }

        [HttpPost("registration-step-1")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegistrationStep1(RegistrationStep1Request registrationStep1Request)
        {
            var registrationStep1Response = await _authService.RegistrationStep1(registrationStep1Request);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = registrationStep1Response.ExpiryTime
            };
            Response.Cookies.Append("registrationToken", registrationStep1Response.Token, cookieOptions);

            return Ok();
        }

        [HttpPost("registration-step-2")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegistrationStep2(RegistrationStep2Request registrationStep2Request)
        {
            var registrationToken = Request.Cookies["registrationToken"];
            await _authService.RegistrationStep2(registrationStep2Request, registrationToken);
            return Ok();
        }

        [HttpPost("upload-profile-image")]
        [ProducesResponseType(typeof(ProfilePictureUploadResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadProfileImage(ProfilePictureUploadRequest profilePictureUploadRequest)
        {
            var registrationToken = Request.Cookies["registrationToken"];
            var profilePictureUrl = await _authService.UploadProfilePicture(profilePictureUploadRequest, registrationToken);
            return Ok(profilePictureUrl);
        }

        [HttpPost("resend-confirmation-code")]
        public async Task<IActionResult> ResendConfirmationCode()
        {
            var registrationToken = Request.Cookies["registrationToken"];
            await _authService.ResendConfirmationCode(registrationToken);
            return Ok();
        }

        [HttpPost("registration-step-3")]
        public async Task<IActionResult> RegistrationStep3(RegistrationStep3Request registrationStep3Request)
        {
            var registrationToken = Request.Cookies["registrationToken"];
            await _authService.RegistrationStep3(registrationStep3Request, registrationToken);

            Response.Cookies.Delete("registrationToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });

            return Ok();
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest refreshTokenRequest)
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var completeAuthResponse = await _authService.RefreshToken(refreshTokenRequest, refreshToken);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = completeAuthResponse.RefreshTokenResponse.ExpiryTime
            };
            Response.Cookies.Append("refreshToken", completeAuthResponse.RefreshTokenResponse.Token, cookieOptions);

            return Ok(completeAuthResponse.AuthResponse);
        }

        [HttpPost("forgotpassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest forgotPasswordRequest)
        {
            await _authService.ForgotPassword(forgotPasswordRequest);
            return Ok();
        }

        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest forgotPasswordRequest)
        {
            await _authService.ResetPassword(forgotPasswordRequest);
            return Ok();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _authService.Logout(User);

            Response.Cookies.Delete("refreshToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });

            return Ok();
        }
    }
}
