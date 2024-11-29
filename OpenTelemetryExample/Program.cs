using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

const string serviceName = "roll_dice_app";

// Configure logging with OpenTelemetry
builder.Logging.AddOpenTelemetry(options =>
{
    options
        .SetResourceBuilder(
            ResourceBuilder.CreateDefault()
                .AddService(serviceName))
        .AddConsoleExporter();
});

// Configure tracing and metrics with OpenTelemetry
builder.Services.AddOpenTelemetry()
      .ConfigureResource(resource => resource.AddService(serviceName))
      .WithTracing(tracing => tracing
          .AddAspNetCoreInstrumentation()
          .AddConsoleExporter())
      .WithMetrics(metrics => metrics
          .AddAspNetCoreInstrumentation()
          .AddPrometheusExporter()); // This should now be available with the new package

var app = builder.Build();

// Endpoint for the dice rolling API
string HandleRollDice([FromServices] ILogger<Program> logger, string? player)
{
    var result = RollDice();

    if (string.IsNullOrEmpty(player))
    {
        logger.LogInformation("Anonymous player is rolling the dice: {result}", result);
    }
    else
    {
        logger.LogInformation("{player} is rolling the dice: {result}", player, result);
    }

    return result.ToString(CultureInfo.InvariantCulture);
}

int RollDice()
{
    return Random.Shared.Next(1, 7);
}

// Map API endpoints
app.MapGet("/rolldice/{player?}", HandleRollDice);

// Expose the /metrics endpoint for Prometheus
app.UseOpenTelemetryPrometheusScrapingEndpoint(); // This replaces MapMetrics

app.Use(async (context, next) =>
{
    await next();

    if (context.Request.Path == "/metrics")
    {
        context.Response.Headers["Content-Type"] = "text/plain; version=0.0.4";
    }
});

app.Run();
