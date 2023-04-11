namespace TimestampMicroservice.Endpoints
{
    public static class TimestampEndpointExtensions
    {
        static public WebApplication MapTimestampService(this WebApplication app)
        {
            app.MapGet("/api/", (TimestampGenerator generator) => Results.Ok(generator.GenerateCurrentTimestamp())).WithOpenApi();
            app.MapGet("/api/{dateString}", (TimestampGenerator generator, string dateString) =>
            {
                if (generator.TryGenerateTimestampFromString(dateString, out Timestamp? timestamp))
                {
                    return Results.Ok(timestamp);
                }

                return Results.BadRequest("Invalid Date");
            }).WithOpenApi();

            return app;
        }

        static public IServiceCollection AddTimestampServices(this IServiceCollection services)
        {
            // TODO: When .NET 8 comes out we can use the built-in TimeProvider
            services.AddSingleton<ITimeProvider, TimeProvider>();
            services.AddSingleton<TimestampGenerator>();
            return services;
        }
    }
}
