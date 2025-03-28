using Sentinel.Api.Application.DTO.Device;
using Sentinel.Common.DTO.Device;
using Sentinel.Common.DTO.Device.Information;

namespace Sentinel.Api.Application.Interfaces
{
    public interface IDeviceRepository
    {
        void Ping(int id);
        DeviceTokenResponse Register(RegisterDeviceDto registerDevice);
        GetDevicesResponse GetDevices(int userId);
        GetDeviceInformationDto GetDeviceInformation(int id);
        void UpdateDeviceInformation(int id, UpdateDeviceInformationDto update);
        StorageInformation GetStorageInfo(int id);
        void UpdateStorageInfo(int id, StorageInformation update);
        SecurityInformation GetSecurityInfo(int id);
        void UpdateSecurityInfo(int id, SecurityInformation update);
        SoftwareInformation GetSoftwareInfo(int id);
        void UpdateSoftwareInfo(int id, SoftwareInformation update);
    }
}
