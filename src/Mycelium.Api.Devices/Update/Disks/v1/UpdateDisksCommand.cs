using Mediator;
using Mycelium.Api.Core.Results;
using Mycelium.Common.DTO.Device;

namespace Mycelium.Api.Devices.Update.Disks.v1;

public sealed record UpdateDisksCommand(int DeviceId, List<DiskDto> Disks) : ICommand<Result>;
