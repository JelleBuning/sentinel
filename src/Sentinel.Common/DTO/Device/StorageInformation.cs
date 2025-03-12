namespace Sentinel.Common.DTO.Device;

public class StorageInformation
{
    public List<DiskInformation> Disks { get; set; } = [];
}

public class DiskInformation
{
    public string? Name { get; set; }
    public bool IsOsDisk { get; set; } = false;
    public double Used {  get; set; }
    public double Size { get; set; }  
}