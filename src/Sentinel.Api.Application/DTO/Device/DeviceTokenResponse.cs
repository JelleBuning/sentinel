namespace Sentinel.Api.Application.DTO.Device;

public class DeviceTokenResponse
{
    public int Id { get; set; }
    public required int OrganisationId { get; set; }
    public required string AccessToken { get; set; }        
    public required string RefreshToken { get; set; }
}