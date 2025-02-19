using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Sentinel.WorkerService.Common.DTO;
using Sentinel.WorkerService.Common.Services.Interfaces;

namespace Sentinel.WorkerService.Common.Services;

public class CredentialManager(IConfiguration configuration) : ICredentialManager
{
    private static readonly string Path =
        System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "credentials.json");

    public async Task SetDeviceDetailsAsync(DeviceRegistrationResponse deviceRegistrationResponse)
    {
        configuration["Id"] = deviceRegistrationResponse.Id.ToString();
        configuration["OrganisationId"] = deviceRegistrationResponse.OrganisationId.ToString();
        configuration["AccessToken"] = deviceRegistrationResponse.AccessToken;
        configuration["RefreshToken"] = deviceRegistrationResponse.RefreshToken;
        await File.WriteAllTextAsync(Path, JsonSerializer.Serialize(deviceRegistrationResponse));
    }

    public async Task SetTokensAsync(DeviceTokenResponse deviceTokenResponse)
    {
        var deviceModel = await GetDeviceDetailsAsync() ?? throw new Exception("No model in storage");
        deviceModel.AccessToken = deviceTokenResponse.AccessToken;
        deviceModel.RefreshToken = deviceTokenResponse.RefreshToken;
        await SetDeviceDetailsAsync(deviceModel);
    }

    public async Task<DeviceRegistrationResponse?> GetDeviceDetailsAsync()
    {
        if (!File.Exists(Path))
        {
            return null;
        }

        var fileContent = await File.ReadAllTextAsync(Path);
        return JsonSerializer.Deserialize<DeviceRegistrationResponse>(fileContent);
    }
}