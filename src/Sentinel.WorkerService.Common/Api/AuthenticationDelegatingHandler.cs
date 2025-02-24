using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Sentinel.WorkerService.Common.DTO;
using Sentinel.WorkerService.Common.Services.Interfaces;

namespace Sentinel.WorkerService.Common.Api;

public class AuthenticationDelegatingHandler(IConfiguration configuration, ICredentialManager credentialManager) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("bearer", configuration["AccessToken"]);

        // No authorization required
        var response = await base.SendAsync(request, cancellationToken);
        if (response.StatusCode != HttpStatusCode.Unauthorized) return response;

        // Authorize
        if (await response.Content.ReadAsStringAsync(cancellationToken) == "Expired JWT")
        {
            var token = await RefreshTokenAsync(cancellationToken);
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", token.AccessToken);
            response = await base.SendAsync(request, cancellationToken);
        }

        return response;
    }
    
    private async Task<DeviceTokenResponse> RefreshTokenAsync(CancellationToken cancellationToken)
    {
        var httpRequestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            Headers = { Authorization = new AuthenticationHeaderValue("bearer", configuration["AccessToken"])},
            RequestUri = new Uri($"{configuration.GetConnectionString("Api")}/devices/auth/refresh"),
            Content = new StringContent(JsonSerializer.Serialize(new 
            {
                AccessToken = configuration["AccessToken"], 
                RefreshToken = configuration["RefreshToken"],
            }), Encoding.UTF8, "application/json")
        };
        var output = await base.SendAsync(httpRequestMessage, cancellationToken);
        output.EnsureSuccessStatusCode();
    
        var deviceTokenResponse = JsonSerializer.Deserialize<DeviceTokenResponse>(await output.Content.ReadAsStringAsync(cancellationToken))!;
        await credentialManager.SetTokensAsync(deviceTokenResponse);
        return deviceTokenResponse;
    }
}