using MassTransit;
using Microsoft.EntityFrameworkCore;
using Notification.API.Features;
using Notification.API.Features.SendBookingConfirmation;
using Notification.API.Features.SendBookingFailure;
using Notification.API.Features.SendCancellation;
using Notification.Application.Abstractions;
using Notification.Infrastructure.Persistence;
using Notification.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var settings = builder.RegisterApiSettings();

// Database
builder.Services.AddDbContext<NotificationDbContext>(options =>
    options.UseSqlServer(settings.ConnectionStrings.SqlServer));

// Repositories
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

// MassTransit with RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    x.AddConsumer<SendBookingConfirmationConsumer>();
    x.AddConsumer<SendBookingFailureConsumer>();
    x.AddConsumer<SendCancellationConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri(settings.ConnectionStrings.RabbitMq));
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddOpenApi();

var app = builder.Build();

// Apply database migrations
await app.MigrateDatabaseAsync<NotificationDbContext>();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapEndpoints();

app.Run();

public partial class Program { }
