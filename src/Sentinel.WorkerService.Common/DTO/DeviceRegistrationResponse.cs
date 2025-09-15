using System.Text.Json.Serialization;

namespace Sentinel.WorkerService.Common.DTO;

public class DeviceRegistrationResponse
{
    public int Id { get; set; }
    public int OrganisationId { get; set; }
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}