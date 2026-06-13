using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mycelium.Api.Application.Commands.Devices.ExecuteSecurityScan;
using Mycelium.Api.Application.Commands.Devices.RequestRemoteAccess;
using Mycelium.Api.Application.Commands.Devices.Restart;
using Mycelium.Api.Application.Queries.Devices.DeviceInformation;
using Mycelium.Api.Application.Queries.Devices.Devices;
using Mycelium.Api.Application.Queries.Devices.SecurityInformation;
using Mycelium.Api.Application.Queries.Devices.SoftwareInformation;
using Mycelium.Api.Application.Queries.Devices.StorageInformation;
using Mycelium.Api.Extensions;

namespace Mycelium.Api.Controllers;

[ApiController]
[Authorize(Roles = "User")]
[Route("/devices")]
public class DeviceAdminController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetDevices()
    {
        var userId = User.GetId();
        var result = await sender.Send(new DevicesQuery(userId));
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDeviceInfo([FromRoute] int id)
    {
        var result = await sender.Send(new DeviceInformationQuery(id));
        return Ok(result);
    }

    [HttpGet("{id}/storage")]
    public async Task<IActionResult> GetStorageInfo([FromRoute] int id)
    {
        var result = await sender.Send(new StorageInformationQuery(id));
        return Ok(result);
    }
        
    [HttpGet("{id}/security")]
    public async Task<IActionResult> GetSecurityInfo([FromRoute] int id)
    {
        var result = await sender.Send(new SecurityInformationQuery(id));
        return Ok(result);
    }
        
    [HttpGet("{id}/software")]
    public async Task<IActionResult> GetSoftwareInfo([FromRoute] int id)
    {
        var result = await sender.Send(new SoftwareInformationQuery(id));
        return Ok(result);
    }

    [HttpPost("remoteAccess/{id}")]
    public async Task<IActionResult> RemoteAccessRequest([FromRoute] int id)
    {
        await sender.Send(new RequestRemoteAccessCommand(id));
        return Ok();
    }
        
    [HttpPost("securityScan/{id}")]
    public async Task<IActionResult> ExecuteSecurityScan([FromRoute] int id)
    {
        await sender.Send(new ExecuteSecurityScanCommand(id));
        return Ok();
    }

    [HttpPost("restartDevice/{id}")]
    public async Task<IActionResult> RestartDevice([FromRoute] int id)
    {
        await sender.Send(new RestartDeviceCommand(id));
        return Ok();
    }
}