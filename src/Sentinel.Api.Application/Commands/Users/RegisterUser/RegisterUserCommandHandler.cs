using Mediator;
using Sentinel.Api.Application.DTO.User;
using Sentinel.Api.Application.Interfaces;

namespace Sentinel.Api.Application.Commands.Users.RegisterUser;

public class RegisterUserCommandHandler(IUserRepository userRepository) : IRequestHandler<RegisterUserCommand>
{
    public async ValueTask<Unit> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var registerDto = new RegisterUserDto
        {
            Email = request.Email,
            Password = request.Password
        };

        await userRepository.Register(registerDto);
        return Unit.Value;
    }
}
