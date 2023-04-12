using Serilog;
using TimestampMicroservice;
using TimestampMicroservice.Endpoints;
using TimestampMicroservice.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTimestampServices();
builder.Services.AddResponseLogging();

builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(builder.Configuration);
});

var app = builder.Build();

ApiDescriptorExtensions.Configure(app.Services.GetRequiredService<IConfiguration>(), app.Services.GetRequiredService<ILogger<ApiDescriptor>>());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();
app.UseResponseLogging();

app.MapTimestampService();

app.Run();

public partial class Program { }