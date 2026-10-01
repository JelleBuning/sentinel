
namespace Mycelium.Api.Auth.Dto;

public class TokenDto
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}
