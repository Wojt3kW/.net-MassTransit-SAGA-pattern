using MassTransit;
using Microsoft.EntityFrameworkCore;
using TripBooking.Saga.API.Features;
using TripBooking.Saga.Persistence;
using TripBooking.Saga.StateMachines;
using TripBooking.Saga.States;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var settings = builder.RegisterApiSettings();

// Database
builder.Services.AddDbContext<TripBookingSagaDbContext>(options =>
    options.UseSqlServer(settings.ConnectionStrings.SqlServer));

// MassTransit with Saga State Machine
builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    // Register the saga state machine with EF Core persistence
    x.AddSagaStateMachine<TripBookingStateMachine, TripBookingSagaState>()
        .EntityFrameworkRepository(r =>
        {
            r.ExistingDbContext<TripBookingSagaDbContext>();
            r.UseSqlServer();
        });

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri(settings.ConnectionStrings.RabbitMq));
        cfg.ConfigureEndpoints(context);
    });
});

// TODO: Add Quartz for scheduling timeouts (will be configured later)

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddOpenApi();

var app = builder.Build();

// Apply database migrations
await app.MigrateDatabaseAsync<TripBookingSagaDbContext>();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "TripBooking Saga API");
    });
    
    // Redirect root to Swagger UI
    app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
}

app.UseHttpsRedirection();

app.MapEndpoints();

app.Run();

public partial class Program { }
