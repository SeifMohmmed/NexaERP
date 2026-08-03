using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NexaERP.BLL.DTOs.Users;
using NexaERP.BLL.Mappings;
using NexaERP.BLL.Services.Abstraction;
using NexaERP.DAL.Database;
using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.BLL.Services.Implementation;

public sealed class AuthService(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    UserManager<IdentityUser> userManager,
    ApplicationDbContext appDbContext,
    ApplicationIdentityDbContext identityDbContext)
    : IAuthService
{
    // Registers a new user.
    public async Task<RegisterResult> RegisterAsync(RegisterUserDto dto)
    {
        // Start a transaction shared between Identity and application databases.
        using IDbContextTransaction transaction =
            await identityDbContext.Database.BeginTransactionAsync();

        // Use the same database connection.
        appDbContext.Database.SetDbConnection(
            identityDbContext.Database.GetDbConnection());

        // Enlist the application context in the transaction.
        await appDbContext.Database.UseTransactionAsync(
            transaction.GetDbTransaction());

        // Create the Identity user.
        var identityUser = new IdentityUser
        {
            Email = dto.Email,
            UserName = dto.Email
        };

        // Create the Identity account.
        IdentityResult result =
            await userManager.CreateAsync(
                identityUser,
                dto.Password);

        // Return validation errors if registration fails.
        if (!result.Succeeded)
        {
            return new RegisterResult
            {
                Succeeded = false,
                Errors = result.Errors.ToDictionary(
                    e => e.Code,
                    e => e.Description)
            };
        }

        // Map the DTO to the application user entity.
        var user = dto.ToEntity();

        // Link the application user to the Identity user.
        user.IdentityId = identityUser.Id;

        // Save the application user.
        await userRepository.AddAsync(user);

        await unitOfWork.SaveChangesAsync();

        // Commit the transaction.
        await transaction.CommitAsync();

        // Return the registration result.
        return new RegisterResult
        {
            Succeeded = true,
            UserId = user.Id
        };
    }
}
