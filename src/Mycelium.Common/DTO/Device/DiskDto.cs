
namespace Mycelium.Common.DTO.Device;

public class DiskDto
{
    public string? Name { get; set; }
    public bool IsOsDisk { get; set; } = false;
    public double Used { get; set; }
    public double Size { get; set; }
    public string? HealthStatus { get; set; }
}
