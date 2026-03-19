namespace Sentinel.Common.DTO.Device;

public class StorageInformationDto
{
    public List<DiskInformationDto> Disks { get; set; } = [];
}

public class DiskInformationDto
{
    public string? Name { get; set; }
    public bool IsOsDisk { get; set; } = false;
    public double Used {  get; set; }
    public double Size { get; set; }  
}