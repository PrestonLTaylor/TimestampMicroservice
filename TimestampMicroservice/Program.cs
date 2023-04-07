using TimestampMicroservice;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// TODO: When .NET 8 comes out we can use the built-in TimeProvider
builder.Services.AddSingleton<ITimeProvider, TimeProvider>();
builder.Services.AddSingleton<TimestampService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/", (TimestampService service) => service.GenerateCurrentTimestamp()).WithOpenApi();
app.MapGet("/api/{dateString}", (TimestampService service, string dateString) => service.GenerateTimestampFromString(dateString)).WithOpenApi();

app.Run();