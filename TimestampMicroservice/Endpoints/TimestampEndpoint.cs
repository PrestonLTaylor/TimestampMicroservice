namespace TimestampMicroservice.Endpoints
{
    public static class TimestampEndpointExtensions
    {
        static public WebApplication MapTimestampService(this WebApplication app)
        {
            app.MapGet("/api/", (TimestampService service) => service.GenerateCurrentTimestamp()).WithOpenApi();
            app.MapGet("/api/{dateString}", (TimestampService service, string dateString) => service.GenerateTimestampFromString(dateString)).WithOpenApi();
            return app;
        }

        static public IServiceCollection AddTimestampServices(this IServiceCollection services)
        {
            // TODO: When .NET 8 comes out we can use the built-in TimeProvider
            services.AddSingleton<ITimeProvider, TimeProvider>();
            services.AddSingleton<TimestampService>();
            return services;
        }
    }
}
