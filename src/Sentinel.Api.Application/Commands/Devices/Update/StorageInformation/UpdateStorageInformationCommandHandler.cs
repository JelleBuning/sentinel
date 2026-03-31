using Mediator;
using Sentinel.Api.Application.Interfaces;

namespace Sentinel.Api.Application.Commands.Devices.Update.StorageInformation;

public class UpdateStorageInformationCommandHandler(IDeviceRepository deviceRepository)
    : IRequestHandler<UpdateStorageInformationCommand>
{
    public ValueTask<Unit> Handle(UpdateStorageInformationCommand request, CancellationToken cancellationToken)
    {
        deviceRepository.UpdateStorageInfo(request.DeviceId, request.StorageInfo);
        return ValueTask.FromResult(Unit.Value);
    }
}
