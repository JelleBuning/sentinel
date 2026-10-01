using System.Net.Http.Headers;
using Mycelium.Api.Auth.Dto;
using Mycelium.Api.Auth.VerifyTotp.v1;
using Mycelium.Api.Users.Register.v1;
using Mycelium.Common.DTO.Device;
using OtpNet;

namespace Mycelium.Api.IntegrationTests.Common;

public static class HttpClientAuthExtensions
{
    extension(HttpClient client)
    {
        public async Task<SignInResponse> AuthenticateUserAsync()
        {
            var signInResponse = await client.SignInUserAsync();
            var totp = new Totp(Base32Encoding.ToBytes(signInResponse.TwoFactorToken), step: 30,
                mode: OtpHashMode.Sha1, totpSize: 6);

            var verifyTotpCommand = new VerifyTotpCommand(
                signInResponse.UserId,
                signInResponse.AuthenticityToken,
                totp.ComputeTotp());

            var verificationResult = await client.PostAsync("/api/v1/auth/users/verify", verifyTotpCommand);
            var userTokenResponse = await verificationResult.Content.DeserializeAsync<TokenDto>()
                                    ?? throw new Exception("verification result was null");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", $"{userTokenResponse.AccessToken}");

            return signInResponse;
        }

        public async Task<DeviceTokenResponse> RegisterDeviceAsync(Guid organisationHash)
        {
            var verificationResult = await client.PostAsync("/api/v1/devices/register", new RegisterDeviceRequest
            {
                Name = "John Doe",
                OrganisationHash = organisationHash,
            });

            var deviceTokenResponse = await verificationResult.Content.DeserializeAsync<DeviceTokenResponse>()
                                      ?? throw new Exception("verification result was null");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", $"{deviceTokenResponse.AccessToken}");

            return deviceTokenResponse;
        }

        private async Task<SignInResponse> SignInUserAsync()
        {
            _ = await client.PostAsync("/api/v1/users/register", new RegisterUserCommand("test@test.com", "password"));

            var signInResult = await client.PostAsync("/api/v1/auth/users/sign_in", new
            {
                Email = "test@test.com",
                Password = "password"
            });

            var response = await signInResult.Content.DeserializeAsync<SignInResponse>();
            if (response == null) throw new Exception("sign_in_response is null");

            return response;
        }
    }
}