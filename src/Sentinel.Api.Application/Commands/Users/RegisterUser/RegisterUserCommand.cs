using Mediator;

namespace Sentinel.Api.Application.Commands.Users.RegisterUser;

public record RegisterUserCommand(string Email, string Password) : IRequest;
