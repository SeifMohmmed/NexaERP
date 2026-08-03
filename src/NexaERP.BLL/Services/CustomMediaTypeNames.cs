namespace NexaERP.BLL.Services;

public static class CustomMediaTypeNames
{
    public static class Application
    {
        // Custom media type for HATEOAS responses.
        public const string HateoasJson = "application/vnd.nexa-erp.hateoas+json";
        public const string HateoasSubType = "hateoas";
    }
}
