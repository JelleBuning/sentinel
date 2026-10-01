
namespace Mycelium.Api.EntityFramework.Entities;

public class Device
{
    public int Id { get; init; }
    public int OrganisationId { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime LastActive { get; set; }
    public required string Name { get; set; }
    public string? RefreshToken { get; set; }

    public DeviceInformation DeviceInformation { get; set; } = new();
    public DeviceSecurity DeviceSecurity { get; set; } = new();
    public List<DeviceDisk> Disks { get; set; } = [];
    public List<DeviceSoftware> Software { get; set; } = [];
}
