using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sentinel.Common.DTO.DeviceInformation;
using Sentinel.WorkerService.Common.Api.Extensions;
using Sentinel.WorkerService.Common.DTO;

namespace Sentinel.WorkerService.Common.Api;

public class SentinelApiService(HttpClient client, IConfiguration configuration, ILogger<SentinelApiService> logger)
{
    public async Task<DeviceRegistrationResponse?> RegisterDeviceAsync(Guid organisationHash, string name,
        CancellationToken cancellationToken)
    {
        try
        {
            var payload = new { organisationHash, name };
            var output = await client.PostAsync("/devices/auth/register", payload, cancellationToken);
            output.EnsureSuccessStatusCode();
            return JsonSerializer.Deserialize<DeviceRegistrationResponse>(
                await output.Content.ReadAsStringAsync(cancellationToken));
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return null;
        }
    }

    public async Task PingAsync()
    {
        var result = await client.PostAsync($"/devices/{configuration["Id"]}/ping");
        result.EnsureSuccessStatusCode();
    }

    public async Task UpdateDeviceInformationAsync(GetDeviceInformationDto getDeviceInformationDto)
    {
        var result = await client.PutAsync($"/devices/{configuration["Id"]}", getDeviceInformationDto);
        result.EnsureSuccessStatusCode();
    }

    public async Task UpdateStorageInformationAsync(StorageInformation storageInformation)
    {
        var result = await client.PutAsync($"/devices/{configuration["Id"]}/storage", storageInformation);
        result.EnsureSuccessStatusCode();
    }

    public async Task UpdateSecurityInformationAsync(SecurityInformation securityInformation)
    {
        var result = await client.PutAsync($"/devices/{configuration["Id"]}/security", securityInformation);
        result.EnsureSuccessStatusCode();
    }

    public async Task UpdateSoftwareInformationAsync(SoftwareInformation softwareInformation)
    {
        var result = await client.PutAsync($"/devices/{configuration["Id"]}/software", softwareInformation);
        result.EnsureSuccessStatusCode();
    }
}