namespace TimestampMicroservice.Endpoints
{
    public sealed class TimestampEndpoint
    {
        static public void DefineEndpoints(WebApplication app)
        {
            app.MapGet("/api/", (TimestampService service) => service.GenerateCurrentTimestamp()).WithOpenApi();
            app.MapGet("/api/{dateString}", (TimestampService service, string dateString) => service.GenerateTimestampFromString(dateString)).WithOpenApi();
        }

        static public void DefineServices(IServiceCollection services)
        {
            // TODO: When .NET 8 comes out we can use the built-in TimeProvider
            services.AddSingleton<ITimeProvider, TimeProvider>();
            services.AddSingleton<TimestampService>();
        }
    }
}
