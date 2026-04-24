using Mycelium.Api.Application.DTO.User;

namespace Mycelium.Api.Application.Interfaces;

public interface IUserRepository
{
    Task Register(RegisterUserDto user);
}