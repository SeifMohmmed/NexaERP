using Microsoft.OpenApi;

namespace NexaERP.API.Extensions;

// Extension methods for configuring Swagger/OpenAPI services.
internal static class SwaggerExtensions
{
    internal static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            // Configure the OpenAPI document metadata.
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "NexaERP API",
                Version = "v1",
            });

            // Use fully qualified type names to avoid schema name conflicts.
            options.CustomSchemaIds(t => t.FullName?.Replace("+", "."));
        });

        return services;
    }
}
