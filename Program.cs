using ExampleOpenTelemetryMongoDB.Business;
using MongoDB.Driver;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .AddSource("MyAspNetService")
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("MyAspNetService"))
            .AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation(options =>
            {
                //options.RecordException = true; // Record exceptions in traces
                options.EnrichWithHttpRequest = (activity, request) =>
                {
                    activity.SetTag("http.request.query_string", request.QueryString.ToString());
                };
            })
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri("http://localhost:4317"); // Adjust the endpoint as needed
                options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
            });

    })
    .WithMetrics(metricProviderBuilder =>
    {
        metricProviderBuilder
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("MyAspNetService"))
            .AddMeter(
                "MongoDB.Driver",
                "Microsoft.AspNetCore.Hosting",
                "Microsoft.AspNetCore.Server.Kestrel",
                "System.Runtime",
                "System.Diagnostics.Process",
                "System.Net.Http"
            )
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri("http://localhost:4317"); // Adjust the endpoint as needed
                options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
            });
    });

builder.Logging.AddOpenTelemetry(options =>
{
    options.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("MyAspNetService"));
    options.IncludeScopes = true; // Wenn du Scopes in Logs willst
    options.ParseStateValues = true;
    options.IncludeFormattedMessage = true;

    // OTLP Exporter (für Collector, Grafana, etc.)
    options.AddOtlpExporter(opt =>
    {
        opt.Endpoint = new Uri("http://localhost:4317"); // oder 4318 für HTTP, je nach Collector
        // opt.Protocol = OtlpExportProtocol.Grpc; // Default: gRPC
    });
});

builder.Services.AddSingleton<IMongoClient>(sp =>
    new MongoClient("mongodb://localhost:27017"));

builder.Services.AddSingleton<WeatherForecastBusiness>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
