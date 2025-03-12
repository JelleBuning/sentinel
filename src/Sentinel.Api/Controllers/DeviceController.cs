using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sentinel.Api.Application.DTO.Device;
using Sentinel.Api.Application.Interfaces;
using Sentinel.Api.Infrastructure.Exceptions;
using Sentinel.Common.DTO.Device;
using Sentinel.Common.DTO.Device.Information;

namespace Sentinel.Api.Controllers;

[ApiController]
[Route("/devices")]
[Authorize(Roles = "Device")]
public class DeviceController(IDeviceRepository deviceRepository, IAuthRepository authRepository) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public IActionResult RegisterDevice(RegisterDeviceDto deviceDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var response = deviceRepository.Register(deviceDto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return new ResponseManager().ReturnResponse(ex);
        }
    }

    [HttpPost("{id}/ping")]
    public IActionResult Ping(int id)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            deviceRepository.Ping(id);
            return Ok();
        }
        catch (Exception ex)
        {
            return new ResponseManager().ReturnResponse(ex);
        }
    }

    [HttpPut("{id}")]
    public IActionResult UpdateDeviceInfo(int id, UpdateDeviceInformationDto updateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            deviceRepository.UpdateDeviceInformation(id, updateDto);
            return Ok();
        }
        catch (Exception ex)
        {
            return new ResponseManager().ReturnResponse(ex);
        }
    }

    [HttpPut("{id}/storage")]
    public IActionResult UpdateStorageInfo(int id, StorageInformation updateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            deviceRepository.UpdateStorageInfo(id, updateDto);
            return Ok();
        }
        catch (Exception ex)
        {
            return new ResponseManager().ReturnResponse(ex);
        }
    }
    
    [HttpPut("{id}/security")]
    public IActionResult UpdateSecurityInfo(int id, SecurityInformation updateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            deviceRepository.UpdateSecurityInfo(id, updateDto);
            return Ok();
        }
        catch (Exception ex)
        {
            return new ResponseManager().ReturnResponse(ex);
        }
    }

    [HttpPut("{id}/software")]
    public IActionResult UpdateSoftwareInfo(int id, SoftwareInformation updateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            deviceRepository.UpdateSoftwareInfo(id, updateDto);
            return Ok();
        }
        catch (Exception ex)
        {
            return new ResponseManager().ReturnResponse(ex);
        }
    }
}