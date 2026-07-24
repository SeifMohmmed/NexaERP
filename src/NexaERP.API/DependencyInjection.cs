using FluentValidation;
using NexaERP.API.Middleware;
using NexaERP.API.Services;
using NexaERP.BLL.DTOs.Customer;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace NexaERP.API;

public static class DependencyInjection
{
    public static WebApplicationBuilder AddApiServices(
        this WebApplicationBuilder builder)
    {
        // Enable dependency injection validation to catch
        // invalid service registrations during startup.
        builder.Host.UseDefaultServiceProvider((context, options) =>
        {
            options.ValidateScopes = true;
            options.ValidateOnBuild = true;
        });

        // Register MVC controllers and configure content negotiation.
        builder.Services.AddControllers(options =>
        {
            // Return HTTP 406 if the requested media type is not supported.
            options.ReturnHttpNotAcceptable = true;
        })
        // Support XML responses in addition to JSON.
        .AddXmlSerializerFormatters();

        // Register FluentValidation validators.
        builder.Services.AddValidatorsFromAssembly(
            typeof(CreateCustomerDto).Assembly,
            includeInternalTypes: true);

        // Configure RFC 7807 Problem Details responses.
        builder.Services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                // Include request ID for troubleshooting.
                context.ProblemDetails.Extensions.TryAdd(
                    "requestId",
                    context.HttpContext.TraceIdentifier);
            };
        });

        builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        // Register OpenAPI/Swagger document generation.
        builder.Services.AddOpenApi();

        // Register HttpContext accessor.
        builder.Services.AddHttpContextAccessor();

        // Register LinkService.
        builder.Services.AddTransient<LinkService>();

        return builder;
    }
    public static WebApplicationBuilder AddObservability(
        this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenTelemetry()
                    .ConfigureResource(resource =>
                        // Register service name in telemetry system
                        resource.AddService(builder.Environment.ApplicationName))
                    .WithTracing(tracing => tracing
                        // Trace outgoing HTTP calls
                        .AddHttpClientInstrumentation()
                        // Trace incoming ASP.NET Core requests
                        .AddAspNetCoreInstrumentation()
                        // Trace PostgreSQL queries
                        .AddNpgsql())
                    .WithMetrics(metrics => metrics
                        // Metrics for outgoing HTTP
                        .AddHttpClientInstrumentation()
                        // Metrics for incoming HTTP
                        .AddAspNetCoreInstrumentation()
                        // Runtime metrics (GC, CPU, etc.)
                        .AddRuntimeInstrumentation())
                    // Export telemetry using OTLP (e.g., to Jaeger, Grafana, etc.)
                    .UseOtlpExporter();

        // Adds OpenTelemetry logging to capture structured logs
        builder.Logging.AddOpenTelemetry(options =>
        {
            // Includes logging scopes (useful for request tracing and correlation IDs)
            options.IncludeScopes = true;

            // Includes the fully formatted log message instead of only template + parameters
            options.IncludeFormattedMessage = true;
        });

        return builder;
    }
}
