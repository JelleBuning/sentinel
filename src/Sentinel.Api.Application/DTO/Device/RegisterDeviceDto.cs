namespace Sentinel.Api.Application.DTO.Device;

public class RegisterDeviceDto
{
    public required Guid OrganisationHash { get; set; }
    public required string Name { get; set; }
}