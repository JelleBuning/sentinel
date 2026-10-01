
namespace Mycelium.Api.EntityFramework.Entities;

public class DeviceInformation
{
    public int Id { get; set; }
    public string? OsName { get; set; }
    public string? OsVersion { get; set; }
    public string? Version { get; set; }
    public string? ProductName { get; set; }
    public string? Processor { get; set; }
    public string? InstalledRam { get; set; }
    public string? GraphicsCard { get; set; }
    public string? Manufacturer { get; set; }
}
