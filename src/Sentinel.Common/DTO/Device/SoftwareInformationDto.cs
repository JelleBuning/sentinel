namespace Sentinel.Common.DTO.Device;

public class SoftwareInformationDto
{
    public List<SoftwareDto> Software { get; set; } = [];
}


public class SoftwareDto
{
    public required string Name { get; set; }
}