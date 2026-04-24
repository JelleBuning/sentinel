using Mycelium.WorkerService.Common.DTO;

namespace Mycelium.WorkerService.Common.Services.Interfaces;

public interface ICredentialManager
{
    public Task SetDeviceDetailsAsync(DeviceRegistrationResponse deviceRegistrationResponse);
    public Task SetTokensAsync(DeviceTokenResponse deviceTokenResponse);
    public Task<DeviceRegistrationResponse?> GetDeviceDetailsAsync();
}