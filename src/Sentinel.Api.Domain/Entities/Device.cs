namespace Sentinel.Api.Domain.Entities;

public class Device
{
    public int Id { get; init; }
    public int OrganisationId { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime LastActive { get; set; }
    public required string Name { get; set; }
    public string? RefreshToken { get; set; }
    
    
    public DeviceDetails? DeviceDetails { get; set; }
    public DeviceSecurity? DeviceSecurity { get; set; }
    public List<DeviceDisk> Disks { get; set; } = [];
    public List<DeviceSoftware> Software { get; set; } = [];
    
}