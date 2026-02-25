using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sentinel.Api.Application.Commands.Auth.Login;
using Sentinel.Api.Application.Commands.Auth.RefreshToken;
using Sentinel.Api.Application.Commands.Auth.VerifyTotp;

namespace Sentinel.Api.Controllers;

[ApiController]
[Route("/auth")]
public class AuthController(ISender sender) : ControllerBase
{
    [HttpPost("users/sign_in")]
    public async Task<IActionResult> Authenticate([FromBody] LoginCommand command)
    {
        var result = await sender.Send(command);
        return Ok(result);
    }

    [HttpPost("users/verify")]
    public async Task<IActionResult> VerifyTotp([FromBody] VerifyTotpCommand command)
    {
        var result = await sender.Send(command);
        return Ok(result);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        var result = await sender.Send(command);
        return Ok(result);
    }
}