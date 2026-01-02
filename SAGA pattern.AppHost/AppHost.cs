var builder = DistributedApplication.CreateBuilder(args);

// Infrastructure - RabbitMQ
var rabbitUsername = builder.AddParameter("rmqUsername", secret: true);
var rabbitPassword = builder.AddParameter("rmqPassword", secret: true);

var rabbitMq = builder.AddRabbitMQ("rabbitMQ", rabbitUsername, rabbitPassword, port: 5672)
            .WithManagementPlugin()
            .WithLifetime(ContainerLifetime.Persistent)
            .WithOtlpExporter();

rabbitUsername.WithParentRelationship(rabbitMq);
rabbitPassword.WithParentRelationship(rabbitMq);

// Services
builder.AddProject<Projects.Trip_API>("trip-api")
    .WaitFor(rabbitMq);

builder.AddProject<Projects.TripBooking_Saga_API>("tripbooking-saga-api")
    .WaitFor(rabbitMq);

builder.AddProject<Projects.FlightBooking_API>("flightbooking-api")
    .WaitFor(rabbitMq);

builder.AddProject<Projects.HotelBooking_API>("hotelbooking-api")
    .WaitFor(rabbitMq);

builder.AddProject<Projects.GroundTransport_API>("groundtransport-api")
    .WaitFor(rabbitMq);

builder.AddProject<Projects.Insurance_API>("insurance-api")
    .WaitFor(rabbitMq);

builder.AddProject<Projects.Payment_API>("payment-api")
    .WaitFor(rabbitMq);

builder.AddProject<Projects.Notification_API>("notification-api")
    .WaitFor(rabbitMq);

builder.Build().Run();
