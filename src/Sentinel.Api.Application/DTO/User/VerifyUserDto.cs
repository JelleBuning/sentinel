namespace Sentinel.Api.Application.DTO.User;

public record VerifyUserDto
{
    public required int UserId { get; init; }
    public required string AuthenticityToken { get; init; }
    public required string OtpAttempt { get; init; }
}