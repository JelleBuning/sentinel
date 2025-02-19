namespace Sentinel.Api.Application.DTO.Token;

public record TokenResponse
{
    
    public required string AccessToken { get; init; }
    
    public required string RefreshToken { get; init; }
}