using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sentinel.Api.Application.Interfaces;
using Sentinel.Api.Infrastructure.Exceptions;

namespace Sentinel.Api.Controllers;


[ApiController]
[Authorize(Roles = "User")]
[Route("/organisations")]
public class OrganisationController(IOrganisationRepository organisationRepository) : Controller
{
    [HttpGet]
    public IActionResult GetOrganisations()
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            return Ok(organisationRepository.GetAll());
        }
        catch (Exception ex)
        {
            return new ResponseManager().ReturnResponse(ex);
        }
    }
}
