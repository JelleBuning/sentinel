using Sentinel.Common.SignalR;

namespace Sentinel.Api.Infrastructure.SignalR.Interfaces;

public interface IDeviceMessageHub
{
    Task RestartDeviceMessage(RestartDeviceMessage restartDeviceMessage);
    Task SecurityScanMessage(SecurityScanMessage securityScanMessage);
    Task<RemoteAccessResponseMessage> RemoteAccessMessage(RemoteAccessMessage remoteAccessMessage);
}