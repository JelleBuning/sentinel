
namespace Mycelium.Common.DTO.Device;

public class RegisterDeviceRequest
{
    public required Guid OrganisationHash { get; set; }
    public required string Name { get; set; }
}
