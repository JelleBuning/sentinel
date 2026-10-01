using Mediator;
using Mycelium.Api.Core.Results;
using Mycelium.Common.DTO.Device.Information;

namespace Mycelium.Api.Devices.Get.ById.v1;

public sealed record GetDeviceInformationQuery(int DeviceId) : IQuery<Result<InformationDto>>;
