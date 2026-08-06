using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.OpenApi;
using NexaERP.API.Middleware;
using NexaERP.API.Services;
using NexaERP.BLL.DTOs.Customer;
using NexaERP.BLL.Services;
using NexaERP.BLL.Services.Abstraction;
using NexaERP.BLL.Services.Implementation;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace NexaERP.API;

public static class DependencyInjection
{
    // Registers API services.
    public static WebApplicationBuilder AddApiServices(
        this WebApplicationBuilder builder)
    {
        // Enable dependency injection validation.
        builder.Host.UseDefaultServiceProvider((context, options) =>
        {
            options.ValidateScopes = true;
            options.ValidateOnBuild = true;
        });

        // Register controllers and configure content negotiation.
        builder.Services.AddControllers(options =>
        {
            // Return HTTP 406 for unsupported media types.
            options.ReturnHttpNotAcceptable = true;
        })
        // Serialize enums as strings.
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters
                .Add(new JsonStringEnumConverter());
        })
        // Enable XML responses.
        .AddXmlSerializerFormatters();

        builder.Services.Configure<MvcOptions>(options =>
        {
            // Get the JSON output formatter.
            var formatter = options.OutputFormatters
                .OfType<SystemTextJsonOutputFormatter>()
                .First();

            // Register the custom HATEOAS media type.
            formatter.SupportedMediaTypes.Add(
                CustomMediaTypeNames.Application.HateoasJson);
        });

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

        // Register exception handlers.
        builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        // Register OpenAPI.
        builder.Services.AddOpenApi();

        // Register HttpContext accessor.
        builder.Services.AddHttpContextAccessor();

        // Register application services.
        builder.Services.AddTransient<LinkService>();
        builder.Services.AddTransient<InvoicePdfService>();
        builder.Services.AddTransient<TokenProvider>();

        // Register business services.
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IRoleService, RoleService>();

        // Configure Swagger.
        AddSwaggerDocumentation(builder.Services);

        // Configure OpenTelemetry.
        builder.AddObservability();

        return builder;
    }

    // Configures OpenTelemetry.
    public static WebApplicationBuilder AddObservability(
        this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenTelemetry()

            // Register the application as a telemetry resource.
            .ConfigureResource(resource =>
                resource.AddService(builder.Environment.ApplicationName))

            // Configure distributed tracing.
            .WithTracing(tracing => tracing
                .AddHttpClientInstrumentation()   // Outgoing requests
                .AddAspNetCoreInstrumentation()   // Incoming requests
                .AddNpgsql())                     // PostgreSQL queries

            // Configure metrics collection.
            .WithMetrics(metrics => metrics
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddRuntimeInstrumentation())

            // Export telemetry via OTLP.
            .UseOtlpExporter();

        // Enable OpenTelemetry logging.
        builder.Logging.AddOpenTelemetry(options =>
        {
            // Include logging scopes.
            options.IncludeScopes = true;

            // Include formatted log messages.
            options.IncludeFormattedMessage = true;
        });

        return builder;
    }

    // Configures Swagger/OpenAPI.
    internal static IServiceCollection AddSwaggerDocumentation(
        this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            // Configure the OpenAPI document.
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "NexaERP API",
                Version = "v1",
            });

            // Prevent schema name conflicts.
            options.CustomSchemaIds(
                t => t.FullName?.Replace("+", "."));
        });

        return services;
    }
}
