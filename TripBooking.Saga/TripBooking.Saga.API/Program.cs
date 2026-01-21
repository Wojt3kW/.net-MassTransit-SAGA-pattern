using MassTransit;
using Microsoft.EntityFrameworkCore;
using Quartz;
using TripBooking.Saga.API.Features;
using TripBooking.Saga.Persistence;
using TripBooking.Saga.StateMachines;
using TripBooking.Saga.States;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var settings = builder.RegisterApiSettings();

builder.Services.AddDbContext<TripBookingSagaDbContext>(options =>
    options.UseSqlServer(settings.ConnectionStrings.SqlServer));

Uri schedulerEndpoint = new Uri("queue:scheduler");

// MassTransit with Saga State Machine - configure BEFORE Quartz
builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    x.AddPublishMessageScheduler();
    x.AddQuartzConsumers();

    x.AddMessageScheduler(schedulerEndpoint);

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
        cfg.UsePublishMessageScheduler();
        cfg.ConfigureEndpoints(context);
        cfg.UseMessageScheduler(schedulerEndpoint);
    });
});

builder.Services
    .AddQuartz(q =>
    {
        q.SchedulerId = "AUTO";
        q.MisfireThreshold = TimeSpan.FromSeconds(10);
        q.UseDefaultThreadPool(tp =>
        {
            tp.MaxConcurrency = 5;
        });
        q.UsePersistentStore(s =>
        {
            s.UseProperties = true;
            s.RetryInterval = TimeSpan.FromSeconds(15);
            s.UseSqlServer(sql =>
            {
                sql.ConnectionString = settings.ConnectionStrings.SqlServer;
                sql.TablePrefix = "QRTZ_";
            });
            s.UseClustering(c =>
            {
                c.CheckinMisfireThreshold = TimeSpan.FromSeconds(20);
                c.CheckinInterval = TimeSpan.FromSeconds(10);
            });
            s.UseSystemTextJsonSerializer();
        });
    })
    .AddQuartzHostedService(options =>
    {
        options.StartDelay = TimeSpan.FromSeconds(5);
        options.WaitForJobsToComplete = true;
    });

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddAuthorization();

var app = builder.Build();

await app.MigrateDatabaseAsync<TripBookingSagaDbContext>();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "TripBooking Saga API");
    });

    app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapEndpoints();

app.Run();

public partial class Program { }
