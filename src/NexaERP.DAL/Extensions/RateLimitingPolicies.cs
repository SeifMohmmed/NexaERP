namespace NexaERP.DAL.Extensions;

public static class RateLimitingPolicies
{
    public const string Auth = "auth";         // Login / Refresh
    public const string Default = "default";   // CRUD
    public const string Heavy = "heavy";       // Reports / PDF / Dashboard
}
