
namespace Mycelium.Api.EntityFramework.Entities;

public class DeviceDisk
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool IsOsDisk { get; set; }
    public double Used { get; set; }
    public double Size { get; set; }
    public string? HealthStatus { get; set; }
}
