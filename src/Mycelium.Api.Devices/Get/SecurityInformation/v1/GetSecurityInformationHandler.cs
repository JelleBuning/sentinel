using Mediator;
using Microsoft.EntityFrameworkCore;
using Mycelium.Api.Core.Results;
using Mycelium.Api.EntityFramework.Persistence;
using Mycelium.Common.DTO.Device;

namespace Mycelium.Api.Devices.Get.SecurityInformation.v1;

public sealed class GetSecurityInformationHandler(AppDbContext dbContext)
    : IQueryHandler<GetSecurityInformationQuery, Result<SecurityDto>>
{
    public async ValueTask<Result<SecurityDto>> Handle(GetSecurityInformationQuery query, CancellationToken cancellationToken)
    {
        var device = await dbContext.Devices
            .Include(d => d.DeviceSecurity)
            .FirstOrDefaultAsync(x => x.Id == query.DeviceId, cancellationToken);

        if (device is null)
        {
            return Result.Failure<SecurityDto>(Error.NotFound("Device not found"));
        }

        var info = device.DeviceSecurity;
        return Result.Success(new SecurityDto
        {
            LastSecurityScanDto = new LastSecurityScanDto
            {
                LastScan = info.LastScan,
                Duration = info.Duration
            },
            AntivirusEnabled = info.AntivirusEnabled,
            LastAntivirusUpdate = info.LastAntivirusUpdate,
            LastAntispywareUpdate = info.LastAntispywareUpdate,
            RealTimeProtectionEnabled = info.RealTimeProtectionEnabled,
            NisEnabled = info.NisEnabled,
            TamperProtectionEnabled = info.TamperProtectionEnabled,
            AntispywareEnabled = info.AntispywareEnabled,
            IsVirtualMachine = info.IsVirtualMachine,
            FirewallSettingsDto = new FirewallSettingsDto
            {
                DomainFirewallEnabled = info.DomainFirewallEnabled,
                PrivateFirewallEnabled = info.PrivateFirewallEnabled,
                PublicFirewallEnabled = info.PublicFirewallEnabled
            }
        });
    }
}
