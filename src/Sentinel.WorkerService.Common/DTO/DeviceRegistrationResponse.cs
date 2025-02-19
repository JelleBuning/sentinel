namespace Sentinel.WorkerService.Common.DTO;

public class DeviceRegistrationResponse
{
    public int Id { get; set; }
    public int OrganisationId { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}