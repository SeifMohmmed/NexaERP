using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NexaERP.DAL.Context;

namespace NexaERP.DAL;

public static class DependencyInjection
{
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("Database"),
                npgsqlOptions =>
                    npgsqlOptions.MigrationsHistoryTable(
                        HistoryRepository.DefaultTableName,
                        Schemas.Application))
            .UseSnakeCaseNamingConvention());

        return services;
    }
}
