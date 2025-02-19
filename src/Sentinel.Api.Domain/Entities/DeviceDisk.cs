namespace Sentinel.Api.Domain.Entities;

public class DeviceDisk
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool IsOsDisk { get; set; } = false;
    public double Used {  get; set; }
    public double Size { get; set; }  
}