using Grpc.Net.Client;

using GrpcClient;

using Microsoft.AspNetCore.Mvc.ApplicationParts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.AddServiceDefaults();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapGet("/greet", (string? name, IConfiguration configuration, CancellationToken cancellationToken) =>
{
    var address = configuration.GetValue<string>("services:grpc-server:http:0") ?? string.Empty;
    using var channel = GrpcChannel.ForAddress(address);

    var client = new Greeter.GreeterClient(channel);
    var message = new HelloRequest
    {
        Name = name ?? "World"
    };
    var reply = client.SayHello(message, cancellationToken: cancellationToken);

    return reply;
});

app.MapGet("/token", (string? user, IConfiguration configuration, CancellationToken cancellationToken) =>
{
    var address = configuration.GetValue<string>("services:grpc-server:http:0") ?? string.Empty;
    using var channel = GrpcChannel.ForAddress(address);

    var client = new Admin.AdminClient(channel);
    var request = new BuscarTokenRequest
    {
        Username = user ?? "user",
        Password = "password"
    };
    var reply = client.BuscarToken(request, cancellationToken: cancellationToken);

    return reply;
});

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
