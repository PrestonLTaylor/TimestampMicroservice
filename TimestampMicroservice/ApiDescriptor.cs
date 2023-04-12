using Serilog;

namespace TimestampMicroservice
{
    public sealed record ApiDescriptor(string Summary, string Description)
    {
        public const string ParentSectionName = "APIDescriptors";
    }

    public static class ApiDescriptorExtensions
    {
        static public void Configure(IConfiguration config, ILogger<ApiDescriptor> logger)
        {
            _config = config;
            _logger = logger;
        }

        static public RouteHandlerBuilder WithDescriptor(this RouteHandlerBuilder builder, string apiPath)
        {
            if (_config is null || _logger is null)
            {
                Log.Warning("Tried to use ApiDescriptorExtenstions.WithDescriptor without calling Configure");
                return builder;
            }

            var descriptorSection = _config.GetSection($"{ApiDescriptor.ParentSectionName}:{apiPath}");
            if (descriptorSection is null)
            {
                _logger.LogWarning("Descriptor for API {ApiPath} was null", apiPath);
                return builder;
            }

            var descriptor = descriptorSection.Get<ApiDescriptor>();
            if (descriptor is null)
            {
                _logger.LogWarning("API descriptor for API {ApiPath} is malformed", apiPath);
                return builder;
            }

            builder.WithSummary(descriptor.Summary)
                .WithDescription(descriptor.Description)
                .WithOpenApi();

            return builder;
        }

        static private IConfiguration? _config;
        static private ILogger<ApiDescriptor>? _logger;
    }
}
