namespace Sentinel.Api.Domain.Entities;

public class Device
{
    public int Id { get; set; }
    public int OrganisationId { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime LastActive { get; set; }
    public string? Name { get; set; }
    public string? RefreshToken { get; set; }
    
    
    public Organisation Organisation { get; init; }
    public DeviceDetails DeviceDetails { get; set; } = new DeviceDetails();
    
    public DeviceSecurity DeviceSecurity { get; set; } = new DeviceSecurity();
    public List<DeviceDisk> Disks { get; set; } = [];
    public List<DeviceSoftware> Software { get; set; } = [];
    
}