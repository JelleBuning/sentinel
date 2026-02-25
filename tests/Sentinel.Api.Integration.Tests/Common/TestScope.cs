using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using OtpNet;
using Sentinel.Api.Application.DTO.Device;
using Sentinel.Api.Application.DTO.Token;
using Sentinel.Api.Application.DTO.User;
using Sentinel.Api.Infrastructure.Persistence;

namespace Sentinel.Api.Integration.Tests.Common;

public sealed class TestScope : IAsyncDisposable
{
    private readonly ApiFixture _fixture;
    private Guid? _organisationHash = null;
    
    public HttpClient Client { get; set; }
    public AppDbContext DbContext => _fixture.GetDbContext();
    public ApiFixture Fixture => _fixture;
     
    public Domain.Entities.Organisation Organisation => DbContext.Organisations
        .Include(x => x.Devices)
        .Include(x => x.Users)
        .Single(x => x.Hash == _organisationHash);
    
    public SignInUserResponse? User { get; private set; }
    
    public TestScope()
    {
        _fixture = new ApiFixture();
        Client = _fixture.CreateClient();
    }
    
    public async Task<TestScope> AuthenticateAsUserAsync()
    {
        var (client, user) = await _fixture.CreateAuthenticatedUserAsync();
        _organisationHash = DbContext.Organisations.Single(x => x.Id == user.OrganisationId).Hash;
        Client = client;
        User = user;
        
        return this;
    }
    
    public async Task<TestScope> AuthenticateAsDeviceAsync()
    {
        if (_organisationHash == null)
            await AddOrganisationAsync();
        
        var (client, device) = await _fixture.CreateAuthenticatedDeviceAsync(_organisationHash ?? throw new Exception("organisation is null"));
        Client = client;
        
        return this;
    }
    
    public async Task<TestScope> AddOrganisationAsync()
    {
        var hash = Guid.NewGuid();
        await Fixture.AddOrganisationAsync(hash);
        _organisationHash = hash;
        return this;
    }
    
    public async Task<TestScope> AddDeviceAsync()
    {
        var device = new Domain.Entities.Device
        {
            Name = "Test Device",
            OrganisationId = Organisation?.Id ?? throw new Exception("Organisation is null. Ensure organisation is added."),
        };
        await Fixture.AddDeviceAsync(device);
        return this;
    }
    
    public async ValueTask DisposeAsync()
    {
        await _fixture.DisposeAsync();
        
    }
}

public static class HttpClientAuthExtensions
{
    extension(HttpClient client)
    {
        public async Task<SignInUserResponse> AuthenticateUserAsync()
        {
            try
            {
                var signInUserResponse = await client.SignInUserAsync();
                var totp = new Totp(Base32Encoding.ToBytes(signInUserResponse.TwoFactorToken), step: 30,
                    mode: OtpHashMode.Sha1, totpSize: 6);
            
                var verifyUserDto = new VerifyUserDto()
                {
                    UserId = signInUserResponse.UserId,
                    AuthenticityToken = signInUserResponse.AuthenticityToken,
                    OtpAttempt = totp.ComputeTotp(),
                };
            
                var verificationResult = await client.PostAsync("/auth/users/verify", verifyUserDto);
                var userTokenResponse = await verificationResult.Content.DeserializeAsync<TokenDto>() 
                    ?? throw new Exception("verification result was null");
                    
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

        public async Task<DeviceTokenResponse> RegisterDeviceAsync(Guid organisationHash)
        {
            try
            {
                var verificationResult = await client.PostAsync("/devices/register", new RegisterDeviceDto
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
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null!;
            }
        }

        private async Task<SignInUserResponse> SignInUserAsync()
        {
            _ = await client.PostAsync("/users/register", new RegisterUserDto 
            { 
                Email = "test@test.com", 
                Password = "password" 
            });
            
            var signInResult = await client.PostAsync("/auth/users/sign_in", new SignInUserDto 
            { 
                Email = "test@test.com", 
                Password = "password" 
            });
            
            var response = await signInResult.Content.DeserializeAsync<SignInUserResponse>();
            if (response == null) throw new Exception("sign_in_response is null");
            
            return response;
        }
    }
}
