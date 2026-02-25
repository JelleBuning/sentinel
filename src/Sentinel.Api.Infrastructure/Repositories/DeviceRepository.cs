using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Sentinel.Api.Application.DTO.Device;
using Sentinel.Api.Application.Interfaces;
using Sentinel.Api.Application.Services.Interfaces;
using Sentinel.Api.Domain.Entities;
using Sentinel.Api.Infrastructure.Exceptions;
using Sentinel.Api.Infrastructure.Persistence;
using Sentinel.Common.DTO.Device;
using Sentinel.Common.DTO.Device.Information;

namespace Sentinel.Api.Infrastructure.Repositories;

public class DeviceRepository(
    AppDbContext dbContext,
    IHttpContextAccessor httpContextAccessor,
    IJwtTokenGenerator tokenGenerator)
    : IDeviceRepository
{
    public DeviceTokenResponse Register(Guid organisationHash, string name)
    {
        var organisation =
            dbContext.Organisations.FirstOrDefault(o => o.Hash == organisationHash) ??
            throw new NotFoundException("Organisation not found");
            
        var device = new Device
        {
            Name = name,
            CreatedOn = DateTime.Now,
            LastActive = DateTime.Now,
            RefreshToken = tokenGenerator.GenerateRefreshToken(),
        };
        organisation.Devices.Add(device);
        dbContext.SaveChanges();
        
        var claims = new List<Claim>
        {
            new("Id", device.Id.ToString()),
            new("Name", device.Name),
            new(ClaimTypes.Role, "Device"),
        };
        return new DeviceTokenResponse
        {
            Id = device.Id,
            OrganisationId = organisation.Id, 
            AccessToken = tokenGenerator.GenerateAccessToken(claims),
            RefreshToken = device.RefreshToken,
        };
    }
        
    public GetDevicesResponse GetDevices(int userId)
    {
        var user = dbContext.Users.Include(user => user.Organisation).Single(x => x.Id == userId);
        var devices = dbContext.Devices.Where(x => x.OrganisationId == user.OrganisationId).ToList();
        return new GetDevicesResponse()
        {
            OrganisationHash = user.Organisation.Hash,
            ActiveDevices =
                devices.Where(x => x.LastActive >= DateTime.Now.AddMinutes(-2)).ToList()
                    .Count, // Devices active in last two minutes
            TotalDevices = devices.Count,
            Status = 0,
            Devices = devices
        };
    }

    public void Ping(int id)
    {
        var device = GetDeviceById(id);
        device.LastActive = DateTime.Now;
        dbContext.SaveChanges();
    }
        
    public GetDeviceInformationDto GetDeviceInformation(int id)
    {
        var device = dbContext.Devices.Include(device => device.DeviceDetails).FirstOrDefault(x => x.Id == id) ??
                     throw new NotFoundException("Device not found");
            
        return new GetDeviceInformationDto
        {
            DeviceName = device.Name,
            OsName = device.DeviceDetails.OsName,
            OsVersion = device.DeviceDetails.OsVersion,
            Version = device.DeviceDetails.Version,
                
            ProductName = device.DeviceDetails.ProductName,
            Processor = device.DeviceDetails.Processor,
            InstalledRam = device.DeviceDetails.InstalledRam,
            GraphicsCard = device.DeviceDetails.GraphicsCard,
            Manufacturer = device.DeviceDetails.Manufacturer,
        };
    }

    public void UpdateDeviceInformation(int id, UpdateDeviceInformationDto updateDto)
    {
        var device = dbContext.Devices.Include(device => device.DeviceDetails).FirstOrDefault(s => s.Id == id) ??
                     throw new NotFoundException("Device not found");

        device.Name = updateDto.DeviceName;
        device.DeviceDetails.OsName = updateDto.OsName;
        device.DeviceDetails.OsVersion = updateDto.OsVersion;
        device.DeviceDetails.Version = updateDto.Version ;
        device.DeviceDetails.Processor = updateDto.Processor;
        device.DeviceDetails.InstalledRam = updateDto.InstalledRam;
        device.DeviceDetails.GraphicsCard = updateDto.GraphicsCard;
        device.DeviceDetails.Manufacturer = updateDto.Manufacturer;

        dbContext.SaveChanges();
    }
        
    public StorageInformationDto GetStorageInfo(int id)
    {
        var device = dbContext.Devices.Include(device => device.Disks).FirstOrDefault(x => x.Id == id) ??
                     throw new NotFoundException("Device not found");

        return new StorageInformationDto()
        {
            Disks = device.Disks.Select(disk => new DiskInformationDto
            {
                Name = disk.Name,
                IsOsDisk = disk.IsOsDisk,
                Used = disk.Used,
                Size = disk.Size
            }).ToList()
        };
    }


    public void UpdateStorageInfo(int id, StorageInformationDto updateDto)
    {
        var device = dbContext.Devices.Include(d => d.Disks).FirstOrDefault(s => s.Id == id) ??
                     throw new NotFoundException("Device not found");

        foreach (var updateDtoDisk in updateDto.Disks)
        {
            var disk = device.Disks.FirstOrDefault(deviceDisk => deviceDisk.Name == updateDtoDisk.Name);
            if (disk == null)
            {
                device.Disks.Add(new DeviceDisk
                {
                    Name = updateDtoDisk.Name,
                    Size = updateDtoDisk.Size,
                    IsOsDisk = updateDtoDisk.IsOsDisk,
                    Used = updateDtoDisk.Used
                });
            }
            else
            {
                disk.Name = updateDtoDisk.Name;
                disk.Size = updateDtoDisk.Size;
                disk.IsOsDisk = updateDtoDisk.IsOsDisk;
                disk.Used = updateDtoDisk.Used;
            }
        }

        dbContext.SaveChanges();
    }

    public SecurityInformationDto GetSecurityInfo(int id)
    {
        var device = dbContext.Devices.Include(d => d.DeviceSecurity).FirstOrDefault(x => x.Id == id) ??
                     throw new NotFoundException("Device not found");

        var securityInfo = device.DeviceSecurity;
        return new SecurityInformationDto
        {
            LastSecurityScanDto = new LastSecurityScanDto
            {
                LastScan = securityInfo.LastScan,
                Duration = securityInfo.Duration
            },
            AntivirusEnabled = securityInfo.AntivirusEnabled,
            LastAntivirusUpdate = securityInfo.LastAntivirusUpdate,
            LastAntispywareUpdate = securityInfo.LastAntispywareUpdate,
            RealTimeProtectionEnabled = securityInfo.RealTimeProtectionEnabled,
            NisEnabled = securityInfo.NisEnabled,
            TamperProtectionEnabled = securityInfo.TamperProtectionEnabled,
            AntispywareEnabled = securityInfo.AntispywareEnabled,
            IsVirtualMachine = securityInfo.IsVirtualMachine,
            FirewallSettingsDto = new FirewallSettingsDto
            {
                DomainFirewallEnabled = securityInfo.DomainFirewallEnabled,
                PrivateFirewallEnabled = securityInfo.PrivateFirewallEnabled,
                PublicFirewallEnabled = securityInfo.PublicFirewallEnabled
            }
        };
    }

    public void UpdateSecurityInfo(int id, SecurityInformationDto updateDto)
    {
        var device = dbContext.Devices.Include(d => d.DeviceSecurity).FirstOrDefault(s => s.Id == id) ??
                     throw new NotFoundException("Device not found");

        device.DeviceSecurity.LastScan = updateDto.LastSecurityScanDto.LastScan;
        device.DeviceSecurity.Duration = updateDto.LastSecurityScanDto.Duration;
        device.DeviceSecurity.AntivirusEnabled = updateDto.AntivirusEnabled;
        device.DeviceSecurity.LastAntivirusUpdate = updateDto.LastAntivirusUpdate;
        device.DeviceSecurity.LastAntispywareUpdate = updateDto.LastAntispywareUpdate;
        device.DeviceSecurity.RealTimeProtectionEnabled = updateDto.RealTimeProtectionEnabled;
        device.DeviceSecurity.NisEnabled = updateDto.NisEnabled;
        device.DeviceSecurity.TamperProtectionEnabled = updateDto.TamperProtectionEnabled;
        device.DeviceSecurity.AntispywareEnabled = updateDto.AntispywareEnabled;
        device.DeviceSecurity.IsVirtualMachine = updateDto.IsVirtualMachine;
        device.DeviceSecurity.DomainFirewallEnabled = updateDto.FirewallSettingsDto.DomainFirewallEnabled;
        device.DeviceSecurity.PrivateFirewallEnabled = updateDto.FirewallSettingsDto.PrivateFirewallEnabled;
        device.DeviceSecurity.PublicFirewallEnabled = updateDto.FirewallSettingsDto.PublicFirewallEnabled;

        dbContext.SaveChanges();
    }

    public SoftwareInformationDto GetSoftwareInfo(int id)
    {
        var device = dbContext.Devices.Include(d => d.Software).FirstOrDefault(s => s.Id == id) ??
                     throw new NotFoundException("Device not found");

        return new SoftwareInformationDto
        {
            Software = device.Software.Select(deviceSoftware => new SoftwareDto
            {
                Name = deviceSoftware.Name
            }).OrderBy(x => x.Name).ToList()
        };
    }

    public void UpdateSoftwareInfo(int id, SoftwareInformationDto updateDto)
    {
        var device = dbContext.Devices.Include(d => d.Software).FirstOrDefault(s => s.Id == id) ??
                     throw new NotFoundException("Device not found");

        foreach (var updateDtoSoftware in updateDto.Software)
        {
            var software = device.Software.FirstOrDefault(x => x.Name == updateDtoSoftware.Name);
            if (software == null)
            {
                device.Software.Add(new DeviceSoftware
                {
                    Name = updateDtoSoftware.Name
                });
            }
            else
            {
                software.Name = updateDtoSoftware.Name;
            }
        }

        dbContext.SaveChanges();
    }

    private Device GetDeviceById(int id)
    {
        if (httpContextAccessor.HttpContext?.User == null)
            throw new UnauthorizedException($"Unauthorized");
        _ = int.TryParse(httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value,
            out var tokenDeviceId);
        if (tokenDeviceId != id)
        {
            throw new ForbiddenException("No access to other devices");
        }

        return dbContext.Devices.FirstOrDefault(x => x.Id == id) ??
               throw new NotFoundException("Device not found");
    }
}