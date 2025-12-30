using Insurance.API.Features;
using Insurance.API.Features.CancelInsurance;
using Insurance.API.Features.IssueInsurance;
using Insurance.Application.Abstractions;
using Insurance.Infrastructure.Persistence;
using Insurance.Infrastructure.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var settings = builder.RegisterApiSettings();

// Database
builder.Services.AddDbContext<InsuranceDbContext>(options =>
    options.UseSqlServer(settings.ConnectionStrings.SqlServer));

// Repositories
builder.Services.AddScoped<IInsurancePolicyRepository, InsurancePolicyRepository>();

// MassTransit with RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    x.AddConsumer<IssueInsuranceConsumer>();
    x.AddConsumer<CancelInsuranceConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri(settings.ConnectionStrings.RabbitMq));
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddOpenApi();

var app = builder.Build();

// Apply database migrations
await app.MigrateDatabaseAsync<InsuranceDbContext>();

app.MapDefaultEndpoints();
app.MapEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();

public partial class Program { }
