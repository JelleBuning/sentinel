
namespace Mycelium.Api.Auth.Dto;

public class SignInResponse
{
    public int UserId { get; set; }
    public int OrganisationId { get; set; }
    public required string AuthenticityToken { get; set; }
    public string? TwoFactorToken { get; set; }
}
