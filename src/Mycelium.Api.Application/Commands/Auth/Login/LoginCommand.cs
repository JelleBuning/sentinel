using Mediator;
using Mycelium.Api.Application.DTO.User;

namespace Mycelium.Api.Application.Commands.Auth.Login;

public record LoginCommand(string Email, string Password) : IRequest<SignInUserResponse>;
