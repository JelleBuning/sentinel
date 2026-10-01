using Mediator;
using Mycelium.Api.Core.Results;
using Mycelium.Common.DTO.Device;

namespace Mycelium.Api.Devices.Get.Disks.v1;

public sealed record GetDisksQuery(int DeviceId) : IQuery<Result<List<DiskDto>>>;
