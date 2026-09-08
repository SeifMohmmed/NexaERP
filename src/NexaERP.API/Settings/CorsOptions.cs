namespace NexaERP.API.Settings;

public sealed class CorsOptions
{
    public const string SectionName = "Cors";
    public const string PolicyName = "NexaERPCorsPolicy";

    public required string[] AllowedOrigins { get; init; }
}
