using GroundTransport.API.Features;
using GroundTransport.API.Features.CancelGroundTransport;
using GroundTransport.API.Features.ReserveGroundTransport;
using GroundTransport.Application.Abstractions;
using GroundTransport.Infrastructure.Persistence;
using GroundTransport.Infrastructure.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var settings = builder.RegisterApiSettings();

// Database
builder.Services.AddDbContext<GroundTransportDbContext>(options =>
    options.UseSqlServer(settings.ConnectionStrings.SqlServer));

// Repositories
builder.Services.AddScoped<ITransportReservationRepository, TransportReservationRepository>();

// MassTransit with RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    x.AddConsumer<ReserveGroundTransportConsumer>();
    x.AddConsumer<CancelGroundTransportConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri(settings.ConnectionStrings.RabbitMq));
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddOpenApi();

var app = builder.Build();

// Apply database migrations
await app.MigrateDatabaseAsync<GroundTransportDbContext>();

app.MapDefaultEndpoints();
app.MapEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();

public partial class Program { }
