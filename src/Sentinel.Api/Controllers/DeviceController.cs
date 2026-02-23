using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sentinel.Api.Application.Commands.Devices.Ping;
using Sentinel.Api.Application.Commands.Devices.Register;
using Sentinel.Api.Application.Commands.Devices.Update.DeviceInformation;
using Sentinel.Api.Application.Commands.Devices.Update.SecurityInformation;
using Sentinel.Api.Application.Commands.Devices.Update.SoftwareInformation;
using Sentinel.Api.Application.Commands.Devices.Update.StorageInformation;
using Sentinel.Api.Application.DTO.Device;
using Sentinel.Api.Extensions;
using Sentinel.Common.DTO.Device;
using Sentinel.Common.DTO.Device.Information;

namespace Sentinel.Api.Controllers;

[ApiController]
[Route("/devices")]
[Authorize(Roles = "Device")]
public class DeviceController(ISender sender) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterDevice([FromBody] RegisterDeviceDto registerDeviceDto)
    {
        var response = await sender.Send(new RegisterDeviceCommand(registerDeviceDto.OrganisationHash, registerDeviceDto.Name));
        return Ok(response);
    }

    [HttpPost("{id}/ping")]
    public async Task<IActionResult> Ping(int id)
    {
        await sender.Send(new PingDeviceCommand(id));
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDeviceInfo(int id, [FromBody] UpdateDeviceInformationDto updateDto)
    {
        await sender.Send(new UpdateDeviceInformationCommand(id, updateDto));
        return Ok();
    }

    [HttpPut("{id}/storage")]
    public async Task<IActionResult> UpdateStorageInfo(int id, [FromBody] StorageInformationDto updateDto)
    {
        await sender.Send(new UpdateStorageInformationCommand(id, updateDto));
        return Ok();
    }
    
    [HttpPut("{id}/security")]
    public async Task<IActionResult> UpdateSecurityInfo(int id, [FromBody] SecurityInformationDto updateDto)
    {
        await sender.Send(new UpdateSecurityInformationCommand(id, updateDto));
        return Ok();
    }

    [HttpPut("{id}/software")]
    public async Task<IActionResult> UpdateSoftwareInfo(int id, [FromBody] SoftwareInformationDto updateDto)
    {
        await sender.Send(new UpdateSoftwareInformationCommand(id, updateDto));
        return Ok();
    }
}