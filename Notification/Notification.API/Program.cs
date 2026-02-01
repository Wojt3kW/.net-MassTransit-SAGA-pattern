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
app.UseCors("AllowDashboard");
app.UseHttpsRedirection();

// Apply database migrations
await app.MigrateDatabaseAsync<NotificationDbContext>();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Notification API");
    });
    
    // Redirect root to Swagger UI
    app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
}

app.UseHttpsRedirection();

app.MapEndpoints();

app.Run();

public partial class Program { }
