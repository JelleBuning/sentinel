using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Sentinel.Api.Application.Interfaces;
using Sentinel.Api.Extensions;
using Sentinel.Api.Infrastructure.Exceptions;
using Sentinel.Api.Infrastructure.SignalR;
using Sentinel.Api.Infrastructure.SignalR.Interfaces;
using Sentinel.Common.Messages;

namespace Sentinel.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = "User")]
    [Route("/devices")]
    public class DeviceAdminController(IHubContext<DeviceMessageHub, IDeviceMessageHub> deviceMessageContext, IDeviceRepository deviceRepository) : ControllerBase
    {
        [HttpGet]
        public IActionResult GetDevices()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = User.GetId();
                return Ok(deviceRepository.GetDevices(userId));
            }
            catch (Exception ex)
            {
                return new ResponseManager().ReturnResponse(ex);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetDeviceInfo([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                return Ok(deviceRepository.GetDeviceInformation(id));
            }
            catch (Exception ex)
            {
                return new ResponseManager().ReturnResponse(ex);
            }
        }

        [HttpGet("{id}/storage")]
        public IActionResult GetStorageInfo([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                return Ok(deviceRepository.GetStorageInfo(id));
            }
            catch (Exception ex)
            {
                return new ResponseManager().ReturnResponse(ex);
            }
        }
        
        [HttpGet("{id}/security")]
        public IActionResult GetSecurityInfo([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                return Ok(deviceRepository.GetSecurityInfo(id));
            }
            catch (Exception ex)
            {
                return new ResponseManager().ReturnResponse(ex);
            }
        }
        
        [HttpGet("{id}/software")]
        public IActionResult GetSoftwareInfo([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                return Ok(deviceRepository.GetSoftwareInfo(id));
            }
            catch (Exception ex)
            {
                return new ResponseManager().ReturnResponse(ex);
            }
        }

        [HttpPost("remoteAccess/{id}")]
        public async Task<IActionResult> RemoteAccessRequest([FromRoute] int id)
        {
            var client = deviceMessageContext.Clients.Client(UserHandler.ConnectedIds.First());
            var response = await client.RemoteAccessMessage(new RemoteAccessMessage());
            return Ok(response);
        }
        
        [HttpPost("securityScan/{id}")]
        public async Task<IActionResult> ExecuteSecurityScan([FromRoute] int id)
        {
            var client = deviceMessageContext.Clients.Client(UserHandler.ConnectedIds.First());
            await client.SecurityScanMessage(new SecurityScanMessage());
            return Ok();
        }

        [HttpPost("restartDevice/{id}")]
        public async Task<IActionResult> RestartDevice([FromRoute] int id)
        {
            await deviceMessageContext.Clients.Client(UserHandler.ConnectedIds.First()).RestartDeviceMessage(new RestartDeviceMessage());
            return Ok();
        }
    }
}
