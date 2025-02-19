namespace Sentinel.Api.Application.DTO.User;

public record SignInUserResponse
{
    public required int UserId { get; init; }
    public int OrganisationId { get; init; }
    public required string AuthenticityToken { get; init; }
    public string? TwoFactorToken { get; init; }
}