namespace Sentinel.Common.DTO.Device.Information;

public class UpdateDeviceInformationDto
{
    // General info
    public required string DeviceName { get; set; }
    public required string OsName { get; set; }
    public required string OsVersion{ get; set; }
    public required string Version { get; set; }

    // Device specs
    public required string ProductName { get; set; }
    public required string Processor { get; set; }
    public required string InstalledRam { get; set; }
    public required string GraphicsCard { get; set; }
    public required string Manufacturer { get; set; }
}