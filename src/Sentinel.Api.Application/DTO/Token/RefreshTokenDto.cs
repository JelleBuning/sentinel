namespace Sentinel.Api.Application.DTO.Token;

public record RefreshTokenDto
{
    
    public required string AccessToken { get; init; }

    
    public required string RefreshToken { get; init; }
};