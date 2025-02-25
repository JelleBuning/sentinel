namespace Sentinel.WorkerService.Common.DTO;

public class DeviceTokenResponse
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}