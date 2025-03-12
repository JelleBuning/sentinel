using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sentinel.Api.Application.DTO.Token;
using Sentinel.Api.Application.DTO.User;
using Sentinel.Api.Application.Interfaces;
using Sentinel.Api.Infrastructure.Exceptions;

namespace Sentinel.Api.Controllers;

[ApiController]
[Route("/auth")]
public class AuthController(IAuthRepository authRepository) : ControllerBase
{
    [HttpPost("users/sign_in")]
    public async Task<IActionResult> Authenticate([FromBody] SignInUserDto singInUserDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        try
        {
            return Ok(await authRepository.AuthenticateAsync(singInUserDto));
        }
        catch (Exception ex)
        {
            return new ResponseManager().ReturnResponse(ex);
        }
    }

    [HttpPost("users/verify")]
    public async Task<IActionResult> VerifyTotp([FromBody] VerifyUserDto verifyUserDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var user = await authRepository.VerifyTotpAsync(verifyUserDto);
            return Ok(await authRepository.GetTokenAsync(user));
        }
        catch (Exception ex)
        {
            return new ResponseManager().ReturnResponse(ex);
        }
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken(TokenDto tokenDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        try
        {
            var response = await authRepository.RefreshTokenAsync(tokenDto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return new ResponseManager().ReturnResponse(ex);
        }
    }
}