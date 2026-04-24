using Mediator;
using Microsoft.AspNetCore.Mvc;
using Mycelium.Api.Application.Commands.Users.RegisterUser;

namespace Mycelium.Api.Controllers;

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
