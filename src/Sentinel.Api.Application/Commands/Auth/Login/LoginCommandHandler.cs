using Mediator;
using Sentinel.Api.Application.DTO.User;
using Sentinel.Api.Application.Interfaces;

namespace Sentinel.Api.Application.Commands.Auth.Login;

public class LoginCommandHandler(IAuthRepository authRepository) : IRequestHandler<LoginCommand, SignInUserResponse>
{
    public async ValueTask<SignInUserResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var signInDto = new SignInUserDto
        {
            Email = request.Email,
            Password = request.Password
        };

        return await authRepository.AuthenticateAsync(signInDto);
    }
}
