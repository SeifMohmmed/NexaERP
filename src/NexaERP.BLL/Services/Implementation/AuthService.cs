using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using NexaERP.BLL.DTOs.Auth;
using NexaERP.BLL.Mappings;
using NexaERP.BLL.Services.Abstraction;
using NexaERP.DAL.Database;
using NexaERP.DAL.Entities;
using NexaERP.DAL.Identity;
using NexaERP.DAL.Repositories.Abstraction;
using NexaERP.DAL.Settings;

namespace NexaERP.BLL.Services.Implementation;

public sealed class AuthService(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork,
    UserManager<IdentityUser> userManager,
    TokenProvider tokenProvider,
    ApplicationDbContext appDbContext,
    ApplicationIdentityDbContext identityDbContext,
    IOptions<JwtAuthOptions> options)
    : IAuthService
{
    // JWT authentication settings.
    private readonly JwtAuthOptions _jwtAuthOptions = options.Value;

    // Registers a new user.
    public async Task<AuthenticationResult> RegisterAsync(RegisterUserDto dto)
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
        IdentityResult createUserResult =
            await userManager.CreateAsync(
                identityUser,
                dto.Password);

        // Return validation errors if registration fails.
        if (!createUserResult.Succeeded)
        {
            return new AuthenticationResult
            {
                Succeeded = false,
                Errors = createUserResult.Errors.ToDictionary(
                    e => e.Code,
                    e => e.Description)
            };
        }

        IdentityResult addToRoleResult =
         await userManager.AddToRoleAsync(
            identityUser,
            Roles.Sales);

        if (!addToRoleResult.Succeeded)
        {
            return new AuthenticationResult
            {
                Succeeded = false,
                Errors = addToRoleResult.Errors.ToDictionary(
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

        // Get the user's roles.
        IList<string> roles =
            await userManager.GetRolesAsync(identityUser);

        // Generate access and refresh tokens.
        var tokenRequest = new TokenRequest(
            identityUser.Id,
            identityUser.Email,
            roles);

        AccessTokenDto accessToken =
            tokenProvider.Create(tokenRequest);

        // Persist the refresh token.
        var refreshToken = new RefreshToken
        {
            Id = Guid.CreateVersion7(),
            UserId = identityUser.Id,
            Token = accessToken.RefreshToken,
            ExpireAtUtc = DateTime.UtcNow.AddDays(
                _jwtAuthOptions.RefreshTokenExiprationDays),
        };

        await refreshTokenRepository.AddAsync(refreshToken);

        await identityDbContext.SaveChangesAsync();

        // Commit the transaction.
        await transaction.CommitAsync();

        // Return the generated tokens.
        return new AuthenticationResult
        {
            Succeeded = true,
            Token = accessToken
        };
    }

    // Authenticates a user and returns JWT tokens.
    public async Task<AuthenticationResult> LoginAsync(LoginUserDto dto)
    {
        // Find the user by email.
        IdentityUser? identityUser =
            await userManager.FindByEmailAsync(dto.Email);

        // Validate the user credentials.
        if (identityUser is null ||
            !await userManager.CheckPasswordAsync(
                identityUser,
                dto.Password))
        {
            return new AuthenticationResult
            {
                Succeeded = false,

                // Return an authentication error.
                Errors = new Dictionary<string, string>
                {
                    ["InvalidCredentials"] = "Invalid email or password."
                }
            };
        }

        IList<string> roles =
            await userManager.GetRolesAsync(identityUser);

        // Generate access and refresh tokens.
        var tokenRequest = new TokenRequest(
            identityUser.Id,
            identityUser.Email!,
            roles);

        AccessTokenDto accessToken =
            tokenProvider.Create(tokenRequest);

        // Persist the refresh token.
        var refreshToken = new RefreshToken
        {
            Id = Guid.CreateVersion7(),
            UserId = identityUser.Id,
            Token = accessToken.RefreshToken,
            ExpireAtUtc = DateTime.UtcNow.AddDays(
                _jwtAuthOptions.RefreshTokenExiprationDays)
        };

        await refreshTokenRepository.AddAsync(refreshToken);

        await identityDbContext.SaveChangesAsync();

        // Return the generated tokens.
        return new AuthenticationResult
        {
            Succeeded = true,
            Token = accessToken
        };
    }

    // Refreshes an expired access token.
    public async Task<AuthenticationResult> RefreshAsync(
        RefreshTokenDto dto)
    {
        // Find the refresh token.
        RefreshToken? refreshToken =
            await refreshTokenRepository.GetByTokenAsync(
                dto.RefreshToken);

        // Validate the refresh token.
        if (refreshToken is null)
        {
            return new AuthenticationResult
            {
                Succeeded = false,
                Errors = new Dictionary<string, string>
                {
                    ["InvalidRefreshToken"] =
                        "Refresh token is invalid."
                }
            };
        }

        // Check whether the refresh token has expired.
        if (refreshToken.ExpireAtUtc <= DateTime.UtcNow)
        {
            return new AuthenticationResult
            {
                Succeeded = false,
                Errors = new Dictionary<string, string>
                {
                    ["ExpiredRefreshToken"] =
                        "Refresh token has expired."
                }
            };
        }

        IList<string> roles =
             await userManager.GetRolesAsync(refreshToken.User);

        // Generate a new access and refresh token.
        var tokenRequest = new TokenRequest(
            refreshToken.User.Id,
            refreshToken.User.Email!,
            roles);

        AccessTokenDto accessToken =
            tokenProvider.Create(tokenRequest);

        // Rotate the refresh token.
        refreshToken.Token = accessToken.RefreshToken;
        refreshToken.ExpireAtUtc = DateTime.UtcNow.AddDays(
            _jwtAuthOptions.RefreshTokenExiprationDays);

        refreshTokenRepository.Update(refreshToken);

        await identityDbContext.SaveChangesAsync();

        // Return the new tokens.
        return new AuthenticationResult
        {
            Succeeded = true,
            Token = accessToken
        };
    }
}
