using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Trip.API.Features;
using Trip.API.Features.CreateTrip;
using Trip.Application.Abstractions;
using Trip.Infrastructure.Persistence;
using Trip.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var settings = builder.RegisterApiSettings();

// Database
builder.Services.AddDbContext<TripDbContext>(options =>
    options.UseSqlServer(settings.ConnectionStrings.SqlServer));

// Repositories
builder.Services.AddScoped<ITripRepository, TripRepository>();

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
builder.Services.AddValidatorsFromAssemblyContaining<CreateTripCommandValidator>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// MassTransit with RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri(settings.ConnectionStrings.RabbitMq));
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddOpenApi();

var app = builder.Build();

// Apply database migrations
await app.MigrateDatabaseAsync<TripDbContext>();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Map all endpoints implementing IEndpoint
app.MapEndpoints();

app.Run();

public partial class Program { }
