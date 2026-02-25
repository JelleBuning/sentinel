namespace Sentinel.Api.Application.DTO.Device;

public class GetDevicesResponse
{
    public int ActiveDevices { get; set; }
    public int TotalDevices { get; set; }
    public int Status { get; set; }
    public Guid OrganisationHash { get; set; }
    public required List<Domain.Entities.Device> Devices { get; set; }
}