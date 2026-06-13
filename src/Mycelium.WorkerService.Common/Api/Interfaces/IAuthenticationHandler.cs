using Mycelium.WorkerService.Common.DTO;

namespace Mycelium.WorkerService.Common.Api.Interfaces;

public interface IAuthenticationHandler
{
    public Task<DeviceRegistrationResponse> EnsureAuthenticatedAsync(Guid organisationHash, string name, CancellationToken cancellationToken);
}