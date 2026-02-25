using Sentinel.WorkerService.Common.DTO;

namespace Sentinel.WorkerService.Common.Api.Interfaces;

public interface IAuthenticationHandler
{
    public Task<DeviceRegistrationResponse> EnsureAuthenticatedAsync(Guid organisationHash, string name, CancellationToken cancellationToken);
}