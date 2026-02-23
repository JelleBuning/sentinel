using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sentinel.Api.Application.Queries.Organisations.GetAllOrganisations;

namespace Sentinel.Api.Controllers;


[ApiController]
[Authorize(Roles = "User")]
[Route("/organisations")]
public class OrganisationController(ISender sender) : Controller
{
    [HttpGet]
    public async Task<IActionResult> GetOrganisations()
    {
        var result = await sender.Send(new GetAllOrganisationsQuery());
        return Ok(result);
    }
}
