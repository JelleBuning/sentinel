using Mediator;
using Microsoft.AspNetCore.Mvc;
using Sentinel.Api.Application.Commands.Users.RegisterUser;

namespace Sentinel.Api.Controllers;

[ApiController]
[Route("/users")]
public class UserController(ISender sender) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        await sender.Send(command);
        return Ok();
    }
}
