using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using RabbitMQ.Client;
using SAGA_pattern.ServiceDefaults.Settings;

namespace Microsoft.Extensions.Hosting;

// Adds common Aspire services: service discovery, resilience, health checks, and OpenTelemetry.
// This project should be referenced by each service project in your solution.
// To learn more about using this project, see https://aka.ms/dotnet/aspire/service-defaults
public static class Extensions
{
    private const string HealthEndpointPath = "/health";
    private const string AlivenessEndpointPath = "/alive";

    public static TBuilder AddServiceDefaults<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.ConfigureNLog();
        builder.ConfigureOpenTelemetry();

        builder.AddDefaultHealthChecks()
                .AddInfrastructureHealthChecks();

        builder.Services.AddServiceDiscovery();

        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            // Turn on resilience by default
            http.AddStandardResilienceHandler();

            // Turn on service discovery by default
            http.AddServiceDiscovery();
        });

        // Uncomment the following to restrict the allowed schemes for service discovery.
        // builder.Services.Configure<ServiceDiscoveryOptions>(options =>
        // {
        //     options.AllowedSchemes = ["https"];
        // });

        return builder;
    }

    public static TBuilder ConfigureNLog<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddNLog();

        return builder;
    }

    public static TBuilder ConfigureOpenTelemetry<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();
            })
            .WithTracing(tracing =>
            {
                tracing.AddSource(builder.Environment.ApplicationName)
                    .AddSource("MassTransit")
                    .AddAspNetCoreInstrumentation(tracing =>
                        // Exclude health check requests from tracing
                        tracing.Filter = context =>
                            !context.Request.Path.StartsWithSegments(HealthEndpointPath)
                            && !context.Request.Path.StartsWithSegments(AlivenessEndpointPath)
                    )
                    // Uncomment the following line to enable gRPC instrumentation (requires the OpenTelemetry.Instrumentation.GrpcNetClient package)
                    //.AddGrpcClientInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSqlClientInstrumentation(options =>
                    {
                        options.SetDbStatementForText = true;
                        options.RecordException = true;
                    })
                    .AddQuartzInstrumentation();
            });

        builder.AddOpenTelemetryExporters();

        return builder;
    }

    private static TBuilder AddOpenTelemetryExporters<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

        if (useOtlpExporter)
        {
            builder.Services.AddOpenTelemetry().UseOtlpExporter();
        }

        // Uncomment the following lines to enable the Azure Monitor exporter (requires the Azure.Monitor.OpenTelemetry.AspNetCore package)
        //if (!string.IsNullOrEmpty(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
        //{
        //    builder.Services.AddOpenTelemetry()
        //       .UseAzureMonitor();
        //}

        return builder;
    }

    public static TBuilder AddDefaultHealthChecks<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddHealthChecks()
            // Add a default liveness check to ensure app is responsive
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

        return builder;
    }

    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        // Adding health checks endpoints to applications in non-development environments has security implications.
        // See https://aka.ms/dotnet/aspire/healthchecks for details before enabling these endpoints in non-development environments.
        if (app.Environment.IsDevelopment())
        {
            // All health checks must pass for app to be considered ready to accept traffic after starting
            app.MapHealthChecks(HealthEndpointPath);

            // Only health checks tagged with the "live" tag must pass for app to be considered alive
            app.MapHealthChecks(AlivenessEndpointPath, new HealthCheckOptions
            {
                Predicate = r => r.Tags.Contains("live")
            });
        }

        return app;
    }

    /// <summary>
    /// Applies pending Entity Framework Core migrations for the specified DbContext.
    /// </summary>
    public static async Task MigrateDatabaseAsync<TDbContext>(this WebApplication app) where TDbContext : DbContext
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<TDbContext>>();

        try
        {
            logger.LogInformation("Applying database migrations for {DbContext}...", typeof(TDbContext).Name);
            await dbContext.Database.MigrateAsync();
            logger.LogInformation("Database migrations applied successfully for {DbContext}.", typeof(TDbContext).Name);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while applying database migrations for {DbContext}.", typeof(TDbContext).Name);
            throw;
        }
    }

    /// <summary>
    /// Registers API settings from configuration and returns the settings instance.
    /// </summary>
    public static ApiSettings RegisterApiSettings<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        var settings = builder.Configuration.Get<ApiSettings>()
            ?? throw new InvalidOperationException("ApiSettings configuration is missing or invalid.");

        builder.Services.AddSingleton(settings);

        return settings;
    }

    /// <summary>
    /// Validates connection strings and adds health checks for SQL Server and RabbitMQ.
    /// </summary>
    /// <returns>Validated connection strings for use in DbContext and MassTransit configuration.</returns>
    private static IHealthChecksBuilder AddInfrastructureHealthChecks<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        var sqlConnectionString = builder.Configuration.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException("Connection string 'SqlServer' is not configured.");
        var rabbitMqConnectionString = builder.Configuration.GetConnectionString("RabbitMq")
            ?? throw new InvalidOperationException("Connection string 'RabbitMq' is not configured.");

        return builder.Services.AddHealthChecks()
            .AddSqlServerHealthCheck(sqlConnectionString)
            .AddRabbitMqHealthCheck(rabbitMqConnectionString);
    }

    /// <summary>
    /// Adds SQL Server health check to verify database connectivity.
    /// </summary>
    private static IHealthChecksBuilder AddSqlServerHealthCheck(this IHealthChecksBuilder builder, string connectionString, string name = "sqlserver")
    {
        return builder.AddSqlServer(
            connectionString: connectionString,
            name: name,
            healthQuery: "SELECT 1;",
            tags: ["database", "ready"]);
    }

    /// <summary>
    /// Adds RabbitMQ health check to verify message broker connectivity.
    /// </summary>
    private static IHealthChecksBuilder AddRabbitMqHealthCheck(this IHealthChecksBuilder builder, string connectionString, string name = "rabbitmq")
    {
        return builder.AddRabbitMQ(
            async _ =>
            {
                var factory = new ConnectionFactory { Uri = new Uri(connectionString) };
                return await factory.CreateConnectionAsync();
            },
            name: name,
            tags: ["rabbitmq", "ready"]);
    }
}
