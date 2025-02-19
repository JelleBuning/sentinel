namespace Sentinel.Api.Domain.Entities;

public sealed class User
{
    public int Id { get; set; }
    public int OrganisationId { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public UserType Type { get; set; }
    public string? AuthenticityToken { get; set; }
    public string? TwoFactorToken { get; set; }
    public DateTime? LastVerified { get; set; } = null;
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }

    public Organisation Organisation { get; init; } = null!;
}


public enum UserType
{
    User,
    Device
}