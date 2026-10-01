using Mediator;
using Mycelium.Api.Core.Results;
using Mycelium.Common.DTO.Device;

namespace Mycelium.Api.Devices.Update.SoftwareInformation.v1;

public sealed record UpdateSoftwareInformationCommand(int DeviceId, List<SoftwareDto> Software) : ICommand<Result>;
