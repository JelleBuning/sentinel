namespace Sentinel.Api.Application.DTO.User;

public record SignInUserDto
{
    
    public required string Email { get; init; }
    
    public required string Password { get; init; }
}