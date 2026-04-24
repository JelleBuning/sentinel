using Mycelium.WorkerService.Common.Api.Interfaces;
using Mycelium.WorkerService.Common.DTO;
using Mycelium.WorkerService.Common.Services.Interfaces;

namespace Mycelium.WorkerService.Common.Api;

public class AuthenticationHandler(MyceliumApiService apiService, ICredentialManager credentialManager)
    : IAuthenticationHandler
{
    public async Task<DeviceRegistrationResponse> EnsureAuthenticatedAsync(Guid organisationHash, string name,
        CancellationToken cancellationToken)
    {
        var deviceToken = await credentialManager.GetDeviceDetailsAsync() 
                          ?? await apiService.RegisterDeviceAsync(organisationHash, name, cancellationToken);
        while (deviceToken == null)
        {
            await Task.Delay(30000, cancellationToken);
            deviceToken = await apiService.RegisterDeviceAsync(organisationHash, name, cancellationToken);
        }

        await credentialManager.SetDeviceDetailsAsync(deviceToken);
        return deviceToken;
    }
}