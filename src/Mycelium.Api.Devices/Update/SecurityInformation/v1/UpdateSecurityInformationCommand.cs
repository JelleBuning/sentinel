using Mediator;
using Mycelium.Api.Core.Results;
using Mycelium.Common.DTO.Device;

namespace Mycelium.Api.Devices.Update.SecurityInformation.v1;

public sealed record UpdateSecurityInformationCommand(int DeviceId, SecurityDto SecurityInfo) : ICommand<Result>;
