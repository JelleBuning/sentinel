using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mycelium.Common.DTO.Device;
using Mycelium.Common.DTO.Device.Information;
using Mycelium.WorkerService.Common.Api.Extensions;

namespace Mycelium.WorkerService.Common.Api;

public class MyceliumApiService(HttpClient client, IConfiguration configuration, ILogger<MyceliumApiService> logger)
{
    public async Task<DeviceTokenResponse?> RegisterDeviceAsync(Guid organisationHash, string name, CancellationToken cancellationToken)
    {
        try
        {
            var payload = new RegisterDeviceRequest { OrganisationHash = organisationHash, Name = name };
            var output = await client.PostAsync("/api/v1/devices/register", payload, cancellationToken);
            output.EnsureSuccessStatusCode();

            return await output.Content.DeserializeAsync<DeviceTokenResponse>(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to register device. OrganisationHash: {OrganisationHash}, Name: {Name}", organisationHash, name);
            return null;
        }
    }

    public async Task PingAsync()
    {
        var result = await client.PostAsync($"/api/v1/devices/{configuration["Id"]}/ping");
        result.EnsureSuccessStatusCode();
    }

    public async Task UpdateDeviceInformationAsync(InformationDto informationDto)
    {
        var result = await client.PutAsync($"/api/v1/devices/{configuration["Id"]}", informationDto);
        result.EnsureSuccessStatusCode();
    }

    public async Task UpdateDiskInformationAsync(List<DiskDto> disks)
    {
        var result = await client.PutAsync($"/api/v1/devices/{configuration["Id"]}/disks", disks);
        result.EnsureSuccessStatusCode();
    }

    public async Task UpdateSecurityInformationAsync(SecurityDto securityDto)
    {
        var result = await client.PutAsync($"/api/v1/devices/{configuration["Id"]}/security", securityDto);
        result.EnsureSuccessStatusCode();
    }

    public async Task UpdateSoftwareInformationAsync(List<SoftwareDto> software)
    {
        var result = await client.PutAsync($"/api/v1/devices/{configuration["Id"]}/software", software);
        result.EnsureSuccessStatusCode();
    }
}