using Microsoft.AspNetCore.HttpLogging;

namespace TimestampMicroservice.Middleware
{
    public static class ResponseLoggingMiddlewareExtensions
    {
        static public IServiceCollection AddResponseLogging(this IServiceCollection services)
        {
            services.AddHttpLogging(logging =>
            {
                logging.LoggingFields = HttpLoggingFields.Response;
            });

            return services;
        }

        static public WebApplication UseResponseLogging(this WebApplication app)
        {
            app.UseWhen(
                context => context.Request.Path.StartsWithSegments("/api"),
                builder => builder.UseHttpLogging()
            );

            return app;
        }
    }
}
