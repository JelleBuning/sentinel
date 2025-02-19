using Microsoft.AspNetCore.Mvc;
using Sentinel.Api.Application.DTO.Token;
using Sentinel.Api.Application.DTO.User;
using Sentinel.Api.Application.Interfaces;
using Sentinel.Api.Infrastructure.Exceptions;

namespace Sentinel.Api.Controllers;

[ApiController]
[Route("/users")]
public class UserController(IUserRepository userRepository) : ControllerBase
{
    [HttpPost("auth/register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto user)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await userRepository.Register(user);
            return Ok();
        }
        catch (Exception ex)
        {
            return new ResponseManager().ReturnResponse(ex);
        }
    }

    [HttpPost("auth/sign_in")]
    public async Task<IActionResult> Authenticate([FromBody] SignInUserDto singInUserDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        try
        {
            return Ok(await userRepository.Authenticate(singInUserDto));
        }
        catch (Exception ex)
        {
            return new ResponseManager().ReturnResponse(ex);
        }
    }

    [HttpPost("auth/verify")]
    public async Task<IActionResult> VerifyTotp([FromBody] VerifyUserDto verifyUserDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var user = await userRepository.VerifyTotp(verifyUserDto);
            return Ok(userRepository.GetToken(user));
        }
        catch (Exception ex)
        {
            return new ResponseManager().ReturnResponse(ex);
        }
    }

    [HttpPost("auth/refresh")]
    public IActionResult Refresh(RefreshTokenDto refreshTokenDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            return Ok(userRepository.RefreshToken(refreshTokenDto));
        }
        catch (Exception ex)
        {
            return new ResponseManager().ReturnResponse(ex);
        }
    }
}
