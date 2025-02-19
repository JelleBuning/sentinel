using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using OtpNet;
using Sentinel.Api.Application.DTO.Token;
using Sentinel.Api.Application.DTO.User;

namespace Sentinel.Api.Integration.Tests.Common;

public static class ClientExtensions
{
    public static async Task<SignInUserResponse> Authenticate(this HttpClient client)
    {
        try
        {
            var signInUserResponse = await client.SignIn();
            var totp = new Totp(Base32Encoding.ToBytes(signInUserResponse.TwoFactorToken), step: 30,
                mode: OtpHashMode.Sha1, totpSize: 6);
            
            // verify
            var verificationResult = await client.PostAsync("/users/auth/verify",
                JsonSerializer.Serialize(new VerifyUserDto()
                {
                    UserId = signInUserResponse.UserId,
                    AuthenticityToken = signInUserResponse.AuthenticityToken,
                    OtpAttempt = totp.ComputeTotp(),
                }));
            var userTokenResponse = JsonSerializer.Deserialize<TokenResponse>(await verificationResult.Content.ReadAsStringAsync()) ??
                throw new Exception("verification result was null");
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

    private static async Task<SignInUserResponse> SignIn(this HttpClient client)
    {
        _ = await client.PostAsync("/users/auth/register", JsonSerializer.Serialize(new RegisterUserDto { Email = "test@test.com", Password = "password", }));
        var signInResult = await client.PostAsync("/users/auth/sign_in", JsonSerializer.Serialize(new SignInUserDto { Email = "test@test.com", Password = "password", }));
        var response = JsonSerializer.Deserialize<SignInUserResponse>(await signInResult.Content.ReadAsStringAsync());
        if(response == null) throw new Exception("sign_in_response is null");
        return response;
    }
    
    public static async Task<HttpResponseMessage> GetAsync(this HttpClient client, string path)
    {
        return await client.GetAsync(path);
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