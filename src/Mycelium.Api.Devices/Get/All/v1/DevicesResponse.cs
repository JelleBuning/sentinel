
namespace Mycelium.Api.Devices.Get.All.v1;

public sealed class DevicesResponse
{
    public Guid OrganisationHash { get; set; }
    public int ActiveDevices { get; set; }
    public int TotalDevices { get; set; }
    public List<DeviceSummaryDto> Devices { get; set; } = [];
}

public sealed class DeviceSummaryDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime LastActive { get; set; }
}
