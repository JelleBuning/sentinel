using Mycelium.Api.Application.DTO.User;
using Mycelium.Api.Application.Interfaces;
using Mycelium.Api.Domain.Entities;
using Mycelium.Api.Infrastructure.Persistence;
using OtpNet;
using Mycelium.Api.Infrastructure.Exceptions;

namespace Mycelium.Api.Infrastructure.Repositories;

public class UserRepository(AppDbContext dbContext) : IUserRepository
{
   public async Task Register(RegisterUserDto user)
    {
        var userExists = dbContext.Users.FirstOrDefault(x => x.Email.ToLower() == user.Email.ToLower());
        if (userExists != null)
        {
            throw new ForbiddenException("Email already in use");
        }

        var key = KeyGeneration.GenerateRandomKey(20);
        var base32String = Base32Encoding.ToString(key);

        var organisation = new Organisation
        {
            Hash = Guid.NewGuid(),
            Users = new List<User>
            {
                new()
                {
                    Email = user.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(user.Password),
                    TwoFactorToken = base32String,
                }
            }
        };

        await dbContext.Organisations.AddAsync(organisation);
        await dbContext.SaveChangesAsync();
    }

}