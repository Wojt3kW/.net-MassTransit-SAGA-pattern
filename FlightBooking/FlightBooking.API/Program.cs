using FlightBooking.API.Features;
using FlightBooking.API.Features.CancelFlight;
using FlightBooking.API.Features.ReserveOutboundFlight;
using FlightBooking.API.Features.ReserveReturnFlight;
using FlightBooking.Application.Abstractions;
using FlightBooking.Infrastructure.Persistence;
using FlightBooking.Infrastructure.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// CORS for Angular dashboard
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowDashboard", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.AddServiceDefaults();

var settings = builder.RegisterApiSettings();

// Database
builder.Services.AddDbContext<FlightBookingDbContext>(options =>
    options.UseSqlServer(settings.ConnectionStrings.SqlServer));

// Repositories
builder.Services.AddScoped<IFlightReservationRepository, FlightReservationRepository>();

// MassTransit with RabbitMQ and Entity Framework Outbox
builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    x.AddConsumer<ReserveOutboundFlightConsumer>();
    x.AddConsumer<ReserveReturnFlightConsumer>();
    x.AddConsumer<CancelFlightConsumer>();

    // Entity Framework Outbox for transactional message delivery
    x.AddEntityFrameworkOutbox<FlightBookingDbContext>(o =>
    {
        o.UseSqlServer();
    });

    // Apply Outbox to all endpoints for message idempotency
    x.AddConfigureEndpointsCallback((context, name, cfg) =>
    {
        cfg.UseEntityFrameworkOutbox<FlightBookingDbContext>(context);
    });

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri(settings.ConnectionStrings.RabbitMq));
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddOpenApi();

var app = builder.Build();
app.UseCors("AllowDashboard");
app.UseHttpsRedirection();

// Apply database migrations
await app.MigrateDatabaseAsync<FlightBookingDbContext>();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "FlightBooking API");
    });
    
    // Redirect root to Swagger UI
    app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
}

app.UseHttpsRedirection();

app.MapEndpoints();

app.Run();

public partial class Program { }
