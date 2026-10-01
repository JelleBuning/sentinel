using Mediator;
using Mycelium.Api.Core.Results;
using Mycelium.Common.DTO.Device;

namespace Mycelium.Api.Devices.Get.SoftwareInformation.v1;

public sealed record GetSoftwareInformationQuery(int DeviceId) : IQuery<Result<List<SoftwareDto>>>;
