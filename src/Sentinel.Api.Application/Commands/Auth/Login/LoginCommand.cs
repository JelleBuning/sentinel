using Mediator;
using Sentinel.Api.Application.DTO.User;

namespace Sentinel.Api.Application.Commands.Auth.Login;

public record LoginCommand(string Email, string Password) : IRequest<SignInUserResponse>;
