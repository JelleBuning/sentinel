
namespace Mycelium.Common.DTO.Device.Information;

/// <summary>
/// Shared shape for GET /devices/{id} and PUT /devices/{id} — one type per direction
/// caused the worker service and API to silently disagree on nullability; this is the
/// single source of truth for both.
/// </summary>
public class InformationDto
{
    // General info
    public string? DeviceName { get; set; }
    public string? OsName { get; set; }
    public string? OsVersion { get; set; }
    public string? Version { get; set; }

    // Device specs
    public string? ProductName { get; set; }
    public string? Processor { get; set; }
    public string? InstalledRam { get; set; }
    public string? GraphicsCard { get; set; }
    public string? Manufacturer { get; set; }
}
