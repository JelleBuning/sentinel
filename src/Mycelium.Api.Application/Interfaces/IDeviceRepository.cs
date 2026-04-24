using Mycelium.Api.Application.DTO.Device;
using Mycelium.Common.DTO.Device;
using Mycelium.Common.DTO.Device.Information;

namespace Mycelium.Api.Application.Interfaces;

public interface IDeviceRepository
{
    void Ping(int id);
    DeviceTokenResponse Register(Guid organisationHash, string name);
    GetDevicesResponse GetDevices(int userId);
    GetDeviceInformationDto GetDeviceInformation(int id);
    void UpdateDeviceInformation(int id, UpdateDeviceInformationDto update);
    StorageInformationDto GetStorageInfo(int id);
    void UpdateStorageInfo(int id, StorageInformationDto update);
    SecurityInformationDto GetSecurityInfo(int id);
    void UpdateSecurityInfo(int id, SecurityInformationDto update);
    SoftwareInformationDto GetSoftwareInfo(int id);
    void UpdateSoftwareInfo(int id, SoftwareInformationDto update);
}