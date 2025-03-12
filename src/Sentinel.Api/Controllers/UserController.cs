using Microsoft.AspNetCore.Mvc;
using Sentinel.Api.Application.DTO.User;
using Sentinel.Api.Application.Interfaces;
using Sentinel.Api.Infrastructure.Exceptions;

namespace Sentinel.Api.Controllers;

[ApiController]
[Route("/users")]
public class UserController(IUserRepository userRepository) : ControllerBase
{
    [HttpPost("register")]
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
}
