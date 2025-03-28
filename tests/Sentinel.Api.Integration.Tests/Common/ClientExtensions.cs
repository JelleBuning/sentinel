using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using OtpNet;
using Sentinel.Api.Application.DTO.Device;
using Sentinel.Api.Application.DTO.Token;
using Sentinel.Api.Application.DTO.User;
using Sentinel.WorkerService.Common.Api.Extensions;

namespace Sentinel.Api.Integration.Tests.Common;

public static class ClientExtensions
{
    public static async Task<SignInUserResponse> AuthenticateUserAsync(this HttpClient client)
    {
        try
        {
            var signInUserResponse = await client.SignInUserAsync();
            var totp = new Totp(Base32Encoding.ToBytes(signInUserResponse.TwoFactorToken), step: 30,
                mode: OtpHashMode.Sha1, totpSize: 6);
            
            // verify
            var verificationResult = await client.PostAsync("/auth/users/verify",
                JsonSerializer.Serialize(new VerifyUserDto()
                {
                    UserId = signInUserResponse.UserId,
                    AuthenticityToken = signInUserResponse.AuthenticityToken,
                    OtpAttempt = totp.ComputeTotp(),
                }));
            var userTokenResponse = await JsonSerializer.DeserializeAsync<TokenDto>(await verificationResult.Content.ReadAsStreamAsync())
                                    ?? throw new Exception("verification result was null");;
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", $"{userTokenResponse.AccessToken}");
            return signInUserResponse;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null!;
        }
    }
    
    public static async Task<DeviceTokenResponse> RegisterDeviceAsync(this HttpClient client, Guid organisationHash)
    {
        try
        {
            var verificationResult = await client.PostAsync("/devices/register", new RegisterDeviceDto
            {
                Name = "John Doe",
                OrganisationHash = organisationHash,
            });
            var deviceTokenResponse = await JsonSerializer.DeserializeAsync<DeviceTokenResponse>(await verificationResult.Content.ReadAsStreamAsync())
                                    ?? throw new Exception("verification result was null");;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{deviceTokenResponse.AccessToken}");
            return deviceTokenResponse;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null!;
        }
    }

    private static async Task<SignInUserResponse> SignInUserAsync(this HttpClient client)
    {
        _ = await client.PostAsync("/users/register", JsonSerializer.Serialize(new RegisterUserDto { Email = "test@test.com", Password = "password", }));
        var signInResult = await client.PostAsync("/auth/users/sign_in", JsonSerializer.Serialize(new SignInUserDto { Email = "test@test.com", Password = "password", }));
        var response = JsonSerializer.Deserialize<SignInUserResponse>(await signInResult.Content.ReadAsStringAsync());
        if(response == null) throw new Exception("sign_in_response is null");
        return response;
    }
    
    private static async Task<HttpResponseMessage> PostAsync(this HttpClient client, string path, string? content)
    {
        
        ByteArrayContent? byteContent = null;
        if (content == null) return await client.PostAsync(path, byteContent);
        var buffer = Encoding.UTF8.GetBytes(content);
        byteContent = new ByteArrayContent(buffer);
        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        return await client.PostAsync(path, byteContent);
    }
}