using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.OpenApi;
using NexaERP.API.Middleware;
using NexaERP.API.Services;
using NexaERP.BLL.DTOs.Customer;
using NexaERP.BLL.Services;
using NexaERP.BLL.Services.Abstraction;
using NexaERP.BLL.Services.Implementation;
using NexaERP.DAL.Extensions;
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

        // Configure Rate Limiting.
        builder.AddRateLimiting();

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

    /// <summary>
    /// Adds and configures rate limiting services for the application. 
    /// Applies:
    /// - Token bucket limiter for authenticated users (per identity).
    /// - Fixed window limiter for anonymous users.
    /// - Custom 429 response with ProblemDetails and Retry-After header.
    /// </summary>
    public static WebApplicationBuilder AddRateLimiting(this WebApplicationBuilder builder)
    {
        // Register the rate limiter middleware/services
        builder.Services.AddRateLimiter(options =>
        {
            // Default status code returned when a request is rejected due to rate limiting
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // Custom handler executed when a request is rejected
            options.OnRejected = async (context, cancellationToken) =>
            {
                // Try to get Retry-After metadata (how long client should wait)
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out TimeSpan retryAfter))
                {
                    // Add Retry-After header in seconds
                    context.HttpContext.Response.Headers.RetryAfter = $"{retryAfter.TotalSeconds}";

                    // Resolve ProblemDetailsFactory to create standardized error response
                    ProblemDetailsFactory problemDetailsFactory = context.HttpContext.RequestServices
                        .GetRequiredService<ProblemDetailsFactory>();

                    // Create RFC7807 ProblemDetails response
                    Microsoft.AspNetCore.Mvc.ProblemDetails problemDetails = problemDetailsFactory.CreateProblemDetails(
                        httpContext: context.HttpContext,
                        statusCode: StatusCodes.Status429TooManyRequests,
                        title: "Too Many Requests",
                        detail: $"Too Many Requests. Please try again after {retryAfter.TotalSeconds} seconds");

                    // Write the response as JSON
                    await context.HttpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
                }
            };

            // Add a named policy called "default"
            options.AddPolicy(
                RateLimitingPolicies.Default, httpContext =>
            {
                // Try to get authenticated user's identity ID (custom extension method)
                string? identityId = httpContext.User.GetIdentityId();

                // If user is authenticated → apply per-user token bucket limiter
                if (!string.IsNullOrWhiteSpace(identityId))
                {
                    return RateLimitPartition.GetTokenBucketLimiter(identityId, _ => new()
                    {
                        // Maximum tokens allowed in the bucket
                        TokenLimit = 60,

                        // How often tokens are replenished
                        ReplenishmentPeriod = TimeSpan.FromMinutes(1),

                        // Number of tokens added each period
                        TokensPerPeriod = 60,

                        // Max queued requests when limit is reached
                        QueueLimit = 5,

                        // Process queued requests in FIFO order
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    });
                }

                // Anonymous users → stricter fixed window limiter
                return RateLimitPartition.GetFixedWindowLimiter(
                    "anonymous",
                    _ => new()
                    {
                        // Max requests allowed per window
                        PermitLimit = 10,

                        // Time window duration
                        Window = TimeSpan.FromMinutes(1),
                    });
            });

            // Authentication endpoints policy.
            options.AddPolicy(
                RateLimitingPolicies.Auth,
                httpContext =>
                {
                    // Use the user ID or IP address as the partition key.
                    string key =
                    httpContext.User.GetIdentityId()
                    ?? httpContext.Connection.RemoteIpAddress?.ToString()
                    ?? "anonymous";

                    return RateLimitPartition.GetFixedWindowLimiter(
                    key,
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    });
                });

            // Heavy operation policy.
            options.AddPolicy(
                RateLimitingPolicies.Heavy,
                httpContext =>
                {
                    // Use the user ID or IP address as the partition key.
                    string key =
                    httpContext.User.GetIdentityId()
                    ?? httpContext.Connection.RemoteIpAddress?.ToString()
                    ?? "anonymous";

                    return RateLimitPartition.GetTokenBucketLimiter(
                    key,
                    _ => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = 10,
                        TokensPerPeriod = 10,
                        ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                        AutoReplenishment = true,
                        QueueLimit = 2,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                    });
                });
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
