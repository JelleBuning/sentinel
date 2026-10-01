using Mediator;
using Microsoft.EntityFrameworkCore;
using Mycelium.Api.Core.Results;
using Mycelium.Api.EntityFramework.Persistence;

namespace Mycelium.Api.Devices.Get.All.v1;

public sealed class GetAllDevicesHandler(AppDbContext dbContext) : IQueryHandler<GetAllDevicesQuery, Result<DevicesResponse>>
{
    public async ValueTask<Result<DevicesResponse>> Handle(GetAllDevicesQuery query, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .Include(u => u.Organisation)
            .FirstOrDefaultAsync(x => x.Id == query.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<DevicesResponse>(Error.NotFound("User not found"));
        }

        var devices = await dbContext.Devices
            .Where(d => d.OrganisationId == user.OrganisationId)
            .Select(d => new DeviceSummaryDto
            {
                Id = d.Id,
                Name = d.Name,
                CreatedOn = d.CreatedOn,
                LastActive = d.LastActive
            })
            .ToListAsync(cancellationToken);

        return Result.Success(new DevicesResponse
        {
            OrganisationHash = user.Organisation.Hash,
            ActiveDevices = devices.Count(d => d.LastActive >= DateTime.Now.AddMinutes(-2)),
            TotalDevices = devices.Count,
            Devices = devices
        });
    }
}
