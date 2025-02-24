using Sentinel.Api.Application.DTO.Device;
using Sentinel.Api.Application.DTO.Token;
using Sentinel.Common.DTO.DeviceInformation;

namespace Sentinel.Api.Application.Interfaces
{
    public interface IDeviceRepository
    {
        void Ping(int id);
        DeviceTokenResponse Register(RegisterDeviceDto registerDevice);
        TokenResponse RefreshToken(RefreshTokenDto refreshTokenDto);
        GetDevicesResponse GetDevices(int userId);
        DeviceInformation GetDeviceInformation(int id);
        void UpdateDeviceInformation(int id, DeviceInformation update);
        StorageInformation GetStorageInfo(int id);
        void UpdateStorageInfo(int id, StorageInformation update);
        SecurityInformation GetSecurityInfo(int id);
        void UpdateSecurityInfo(int id, SecurityInformation update);
        SoftwareInformation GetSoftwareInfo(int id);
        void UpdateSoftwareInfo(int id, SoftwareInformation update);
    }
}
