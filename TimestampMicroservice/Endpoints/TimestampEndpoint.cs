namespace TimestampMicroservice.Endpoints
{
    public static class TimestampEndpointExtensions
    {
        static public WebApplication MapTimestampService(this WebApplication app)
        {
            app.MapGet("/api/", (TimestampGenerator generator) => Results.Ok(generator.GenerateCurrentTimestamp()))
                .WithDescriptor("/api");

            app.MapGet("/api/{unixTimestamp:long:min(0)}", (TimestampGenerator generator, ulong unixTimestamp) => 
                Results.Ok(generator.GenerateTimestampFromUnixTimestamp(unixTimestamp)))
                .WithDescriptor("/api/{unixTimestamp}");

            app.MapGet("/api/{dateString}", (TimestampGenerator generator, string dateString) =>
            {
                if (generator.TryGenerateTimestampFromDateString(dateString, out Timestamp? timestamp))
                {
                    return Results.Ok(timestamp);
                }

                return Results.BadRequest("Invalid Date");
            })
                .WithDescriptor("/api/{dateString}");

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
