namespace Sentinel.Common.DTO.Device;

public class SoftwareInformation
{
    public List<Software> Software { get; set; } = [];
}


public class Software
{
    public required string Name { get; set; }
}