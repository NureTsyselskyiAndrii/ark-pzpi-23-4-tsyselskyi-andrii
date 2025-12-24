using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using SafeDose.Application.Contracts.Identity;
using SafeDose.Application.Exceptions;
using SafeDose.Application.Models.Identity.GoogleAuth;
using SafeDose.Application.Models.Identity.Settings;
using System.Text.Json;

namespace SafeDose.Identity.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly GoogleSettings _googleSettings;
        private readonly HttpClient _httpClient;
        public GoogleAuthService(IOptions<GoogleSettings> googleSettings, HttpClient httpClient)
        {
            _googleSettings = googleSettings.Value;
            _httpClient = httpClient;
        }
        public async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(GoogleAuthRequest authRequest)
        {
            try
            {
                GoogleJsonWebSignature.Payload payload;
                if (authRequest.IsMobile)
                {
                    payload = await GoogleJsonWebSignature.ValidateAsync(
                        authRequest.CodeOrIdToken,
                        new GoogleJsonWebSignature.ValidationSettings
                        {
                            Audience = new List<string>
                            {
                                _googleSettings.WebClientId,
                                _googleSettings.AndroidClientId,
                                _googleSettings.IOSClientId
                            }
                        }
                    );
                }
                else
                {
                    var tokenResponse = await ExchangeCodeForTokens(authRequest.CodeOrIdToken);

                    if (string.IsNullOrEmpty(tokenResponse.IdToken))
                    {
                        throw new BadRequestException("Failed to retrieve ID token from Google.");
                    }

                    payload = await GoogleJsonWebSignature.ValidateAsync(
                        tokenResponse.IdToken,
                        new GoogleJsonWebSignature.ValidationSettings
                        {
                            Audience = new List<string>
                            {
                                _googleSettings.WebClientId
                            }
                        }
                    );
                }

                return payload;
            }
            catch (Exception)
            {
                throw new BadRequestException("Code is invalid.");
            }
        }

        private async Task<GoogleTokenResponse> ExchangeCodeForTokens(string code)
        {
            var parameters = new Dictionary<string, string>
            {
                { "code", code },
                { "client_id", _googleSettings.WebClientId },
                { "client_secret", _googleSettings.ClientSecret },
                { "redirect_uri", _googleSettings.RedirectUri },
                { "grant_type", "authorization_code" }
            };

            var content = new FormUrlEncodedContent(parameters);
            var response = await _httpClient.PostAsync("https://oauth2.googleapis.com/token", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new BadRequestException("Failed to exchange code for token.");
            }

            var json = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<GoogleTokenResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return tokenResponse!;
        }
    }
}
