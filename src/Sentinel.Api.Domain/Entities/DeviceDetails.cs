namespace Sentinel.Api.Domain.Entities;

public class DeviceDetails
{
    public int Id { get; set; }
    public string? OsName;
    public string? OsVersion;
    public string? Version { get; set; }
    public string? ProductName { get; set; }
    public string? Processor { get; set; }
    public string? InstalledRam { get; set; }
    public string? GraphicsCard { get; set; }
    public string? Manufacturer { get; set; }
}