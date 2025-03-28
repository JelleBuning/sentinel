namespace Sentinel.Api.Application.DTO.User;

public record RegisterUserDto
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}