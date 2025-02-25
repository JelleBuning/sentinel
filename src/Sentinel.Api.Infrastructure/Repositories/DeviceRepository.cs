using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Sentinel.Api.Application.DTO.Device;
using Sentinel.Api.Application.DTO.Token;
using Sentinel.Api.Application.Interfaces;
using Sentinel.Api.Application.Services.Interfaces;
using Sentinel.Api.Domain.Entities;
using Sentinel.Api.Infrastructure.Exceptions;
using Sentinel.Api.Infrastructure.Persistence;
using Sentinel.Common.DTO.DeviceInformation;

namespace Sentinel.Api.Infrastructure.Repositories
{
    public class DeviceRepository(
        AppDbContext dbContext,
        IHttpContextAccessor httpContextAccessor,
        ITokenService tokenService)
        : IDeviceRepository
    {
        public DeviceTokenResponse Register(RegisterDeviceDto registerDeviceDto)
        {
            var organisation =
                dbContext.Organisations.FirstOrDefault(o => o.Hash == registerDeviceDto.OrganisationHash) ??
                throw new Exception("Organisation not found", new NotFoundException());
            
            var device = new Device
            {
                Name = registerDeviceDto.Name,
                CreatedOn = DateTime.Now,
                LastActive = DateTime.Now,
                RefreshToken = tokenService.GenerateRefreshToken(),
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
                AccessToken = tokenService.GenerateAccessToken(claims),
                RefreshToken = device.RefreshToken,
            };
        }

        public TokenResponse RefreshToken(RefreshTokenDto refreshTokenDto) // TODO: fix
        {
            var principal = tokenService.GetPrincipalFromExpiredToken(refreshTokenDto.AccessToken);
            var claimId = principal.Claims.FirstOrDefault(c => c.Type == "Id")!.Value;
            var device = dbContext.Devices.SingleOrDefault(x => x.Id.ToString() == claimId);
            if (device == null || device.RefreshToken != refreshTokenDto.RefreshToken)
            {
                throw new Exception("Invalid refresh token", new BadRequestException());
            }

            var newAccessToken = tokenService.GenerateAccessToken(principal.Claims);
            var newRefreshToken = tokenService.GenerateRefreshToken();
            device.RefreshToken = newRefreshToken;
            dbContext.SaveChanges();
            
            return new TokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
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
                         throw new Exception("Device not found", new NotFoundException());
            
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
                         throw new Exception("Device not found", new NotFoundException());

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
        
        public StorageInformation GetStorageInfo(int id)
        {
            var device = dbContext.Devices.Include(device => device.Disks).FirstOrDefault(x => x.Id == id) ??
                         throw new Exception("Device not found", new NotFoundException());

            return new StorageInformation()
            {
                Disks = device.Disks.Select(disk => new DiskInformation
                {
                    Name = disk.Name,
                    IsOsDisk = disk.IsOsDisk,
                    Used = disk.Used,
                    Size = disk.Size
                }).ToList()
            };
        }


        public void UpdateStorageInfo(int id, StorageInformation updateDto)
        {
            var device = dbContext.Devices.Include(d => d.Disks).FirstOrDefault(s => s.Id == id) ??
                         throw new Exception("Device not found", new NotFoundException());

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

        public SecurityInformation GetSecurityInfo(int id)
        {
            var device = dbContext.Devices.Include(d => d.DeviceSecurity).FirstOrDefault(x => x.Id == id) ??
                         throw new Exception("Device not found", new NotFoundException());

            var securityInfo = device.DeviceSecurity;
            return new SecurityInformation
            {
                LastSecurityScan = new LastSecurityScan
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
                FirewallSettings = new FirewallSettings
                {
                    DomainFirewallEnabled = securityInfo.DomainFirewallEnabled,
                    PrivateFirewallEnabled = securityInfo.PrivateFirewallEnabled,
                    PublicFirewallEnabled = securityInfo.PublicFirewallEnabled
                }
            };
        }

        public void UpdateSecurityInfo(int id, SecurityInformation updateDto)
        {
            var device = dbContext.Devices.Include(d => d.DeviceSecurity).FirstOrDefault(s => s.Id == id) ??
                         throw new Exception("Device not found", new NotFoundException());

            device.DeviceSecurity.LastScan = updateDto.LastSecurityScan.LastScan;
            device.DeviceSecurity.Duration = updateDto.LastSecurityScan.Duration;
            device.DeviceSecurity.AntivirusEnabled = updateDto.AntivirusEnabled;
            device.DeviceSecurity.LastAntivirusUpdate = updateDto.LastAntivirusUpdate;
            device.DeviceSecurity.LastAntispywareUpdate = updateDto.LastAntispywareUpdate;
            device.DeviceSecurity.RealTimeProtectionEnabled = updateDto.RealTimeProtectionEnabled;
            device.DeviceSecurity.NisEnabled = updateDto.NisEnabled;
            device.DeviceSecurity.TamperProtectionEnabled = updateDto.TamperProtectionEnabled;
            device.DeviceSecurity.AntispywareEnabled = updateDto.AntispywareEnabled;
            device.DeviceSecurity.IsVirtualMachine = updateDto.IsVirtualMachine;
            device.DeviceSecurity.DomainFirewallEnabled = updateDto.FirewallSettings.DomainFirewallEnabled;
            device.DeviceSecurity.PrivateFirewallEnabled = updateDto.FirewallSettings.PrivateFirewallEnabled;
            device.DeviceSecurity.PublicFirewallEnabled = updateDto.FirewallSettings.PublicFirewallEnabled;

            dbContext.SaveChanges();
        }

        public SoftwareInformation GetSoftwareInfo(int id)
        {
            var device = dbContext.Devices.Include(d => d.Software).FirstOrDefault(s => s.Id == id) ??
                         throw new Exception("Device not found", new NotFoundException());

            return new SoftwareInformation
            {
                Software = device.Software.Select(deviceSoftware => new Software
                {
                    Name = deviceSoftware.Name
                }).OrderBy(x => x.Name).ToList()
            };
        }

        public void UpdateSoftwareInfo(int id, SoftwareInformation updateDto)
        {
            var device = dbContext.Devices.Include(d => d.Software).FirstOrDefault(s => s.Id == id) ??
                         throw new Exception("Device not found", new NotFoundException());

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
                throw new Exception($"Unauthorized", new UnauthorizedException());
            _ = int.TryParse(httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value,
                out var tokenDeviceId);
            if (tokenDeviceId != id)
            {
                throw new Exception("No access to other devices", new ForbiddenException());
            }

            return dbContext.Devices.FirstOrDefault(x => x.Id == id) ??
                   throw new Exception("Device not found", new NotFoundException());
        }
    }
}