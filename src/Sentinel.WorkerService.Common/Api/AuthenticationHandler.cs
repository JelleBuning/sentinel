using Sentinel.WorkerService.Common.Api.Interfaces;
using Sentinel.WorkerService.Common.DTO;
using Sentinel.WorkerService.Common.Services.Interfaces;

namespace Sentinel.WorkerService.Common.Api;

public class AuthenticationHandler(SentinelApiService apiService, ICredentialManager credentialManager)
    : IAuthenticationHandler
{
    public async Task<DeviceRegistrationResponse> EnsureAuthenticated(Guid organisationHash, string name,
        CancellationToken cancellationToken)
    {
        var deviceToken = await credentialManager.GetDeviceDetailsAsync() ?? await apiService.RegisterDeviceAsync(organisationHash, name, cancellationToken);
        while (deviceToken == null)
        {
            await Task.Delay(30000, cancellationToken);
            deviceToken = await apiService.RegisterDeviceAsync(organisationHash, name, cancellationToken);
        }

        await credentialManager.SetDeviceDetailsAsync(deviceToken);
        return deviceToken;
    }
}