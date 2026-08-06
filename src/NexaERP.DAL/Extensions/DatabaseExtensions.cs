using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NexaERP.DAL.Authorization;
using NexaERP.DAL.Database;
using NexaERP.DAL.Entities;
using NexaERP.DAL.Identity;
using NexaERP.DAL.Repositories.Abstraction;

namespace NexaERP.DAL.Extensions;

/// <summary>
/// Contains extension methods related to database initialization.
/// </summary>
public static class DatabaseExtensions
{
    /// <summary>
    /// Applies pending Entity Framework Core migrations automatically at application startup.
    /// Ensures database schema is up to date with current models.
    /// </summary>
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        // Create a scoped service provider
        // Required because DbContext is registered as scoped service
        using IServiceScope scope = app.Services.CreateScope();

        // Resolve ApplicationDbContext from DI container
        await using ApplicationDbContext applicationDbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Resolve ApplicationDbContext from DI container
        await using ApplicationIdentityDbContext identityDbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();

        try
        {
            // Apply all pending migrations
            await applicationDbContext.Database.MigrateAsync();
            app.Logger.LogInformation("Application database migrations applied successfully.");

            await identityDbContext.Database.MigrateAsync();
            app.Logger.LogInformation("Identity database migrations applied successfully.");
        }
        catch (Exception ex)
        {
            // Log migration failure
            app.Logger.LogError(ex, "An error occurred while applying database migrations.");

            // Re-throw exception so application fails fast
            throw;
        }
    }

    // Extension method to seed initial roles when the application starts
    public static async Task SeedInitialDataAsync(this WebApplication app)
    {
        // Create a scoped service provider to resolve scoped services like RoleManager
        await using AsyncServiceScope scope = app.Services.CreateAsyncScope();

        // Resolve ASP.NET Identity RoleManager to manage roles
        RoleManager<IdentityRole> roleManager =
            scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        try
        {
            string[] roles =
            [
                Roles.Admin,
                Roles.Sales,
                Roles.Purchasing,
                Roles.Warehouse,
                Roles.Accountant,
                Roles.HR
            ];

            foreach (string roleName in roles)
            {
                IdentityRole? role =
                    await roleManager.FindByNameAsync(roleName);

                if (role is null)
                {
                    role = new IdentityRole(roleName);

                    IdentityResult createRoleResult =
                        await roleManager.CreateAsync(role);

                    if (!createRoleResult.Succeeded)
                    {
                        throw new InvalidOperationException(
                            string.Join(
                                Environment.NewLine,
                                createRoleResult.Errors.Select(e => e.Description)));
                    }
                }

                if (!RoleClaims.Map.TryGetValue(
                    roleName,
                    out IReadOnlyCollection<string>? permissions))
                {
                    continue;
                }

                IList<Claim> existingClaims =
                    await roleManager.GetClaimsAsync(role);

                foreach (string permission in permissions)
                {
                    if (existingClaims.Any(c =>
                        c.Type == JwtCustomClaimNames.Permission &&
                        c.Value == permission))
                    {
                        continue;
                    }

                    IdentityResult addClaimResult =
                        await roleManager.AddClaimAsync(
                            role,
                            new Claim(
                                JwtCustomClaimNames.Permission,
                                permission));

                    if (!addClaimResult.Succeeded)
                    {
                        throw new InvalidOperationException(
                            $"Failed to assign permission '{permission}' to role '{roleName}'.");
                    }
                }
            }

            // Log success message
            app.Logger.LogInformation("Roles created successfully");
        }
        catch (Exception ex)
        {
            // Log error if something goes wrong during seeding
            app.Logger.LogError(ex, "An error occurred while seeding initial data");
            throw;
        }
    }

    public static async Task SeedAdminUserAsync(this WebApplication app)
    {
        await using AsyncServiceScope scope =
            app.Services.CreateAsyncScope();

        UserManager<IdentityUser> userManager =
            scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        IUserRepository userRepository =
            scope.ServiceProvider.GetRequiredService<IUserRepository>();

        IUnitOfWork unitOfWork =
            scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        ApplicationDbContext appDbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        ApplicationIdentityDbContext identityDbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();

        const string adminEmail = "admin@nexaerp.com";
        const string adminPass = "Admin@123";

        try
        {
            // Check whether the admin user already exists.
            IdentityUser? identityUser =
                await userManager.FindByEmailAsync(adminEmail);

            if (identityUser is not null)
            {
                return;
            }

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
            identityUser = new IdentityUser
            {
                Email = adminEmail,
                UserName = adminEmail,
                EmailConfirmed = true
            };

            IdentityResult createUserResult =
                await userManager.CreateAsync(
                    identityUser,
                    adminPass);

            if (!createUserResult.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join(
                        Environment.NewLine,
                        createUserResult.Errors.Select(e => e.Description)));
            }

            // Assign the Admin role.
            IdentityResult addToRoleResult =
                await userManager.AddToRoleAsync(
                    identityUser,
                    Roles.Admin);

            if (!addToRoleResult.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join(
                        Environment.NewLine,
                        addToRoleResult.Errors.Select(e => e.Description)));
            }

            // Create the application user.
            User user = new()
            {
                Id = Guid.CreateVersion7(),
                FirstName = "System",
                LastName = "Administrator",
                Email = adminEmail,
                IdentityId = identityUser.Id,
                CreatedAtUtc = DateTime.UtcNow
            };

            await userRepository.AddAsync(user);

            await unitOfWork.SaveChangesAsync();

            // Commit the transaction.
            await transaction.CommitAsync();

            app.Logger.LogInformation(
                "Default admin user created successfully.");
        }
        catch (Exception ex)
        {
            app.Logger.LogError(
                ex,
                "An error occurred while seeding the default admin user.");

            throw;
        }
    }
}
