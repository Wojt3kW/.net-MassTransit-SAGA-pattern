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

// Observability - Jaeger for distributed tracing
//var jaeger = builder.AddContainer("jaeger", "jaegertracing/all-in-one", "latest")
//    .WithHttpEndpoint(port: 16686, targetPort: 16686, name: "ui")
//    .WithEndpoint(port: 4317, targetPort: 4317, name: "otlp-grpc", scheme: "http")
//    .WithEndpoint(port: 4318, targetPort: 4318, name: "otlp-http", scheme: "http")
//    .WithEnvironment("COLLECTOR_OTLP_ENABLED", "true")
//    .WithLifetime(ContainerLifetime.Persistent);

//// Get Jaeger OTLP endpoint for services
//var jaegerOtlpEndpoint = jaeger.GetEndpoint("otlp-grpc");

// Services - all configured to send telemetry to Jaeger
builder.AddProject<Projects.Trip_API>("trip-api")
    .WaitFor(rabbitMq)
    //.WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", jaegerOtlpEndpoint)
    ;

builder.AddProject<Projects.TripBooking_Saga_API>("tripbooking-saga-api")
    .WaitFor(rabbitMq)
    //.WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", jaegerOtlpEndpoint)
    ;

builder.AddProject<Projects.FlightBooking_API>("flightbooking-api")
    .WaitFor(rabbitMq)
    //.WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", jaegerOtlpEndpoint)
    ;

builder.AddProject<Projects.HotelBooking_API>("hotelbooking-api")
    .WaitFor(rabbitMq)
    //.WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", jaegerOtlpEndpoint)
    ;

builder.AddProject<Projects.GroundTransport_API>("groundtransport-api")
    .WaitFor(rabbitMq)
    //.WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", jaegerOtlpEndpoint)
    ;

builder.AddProject<Projects.Insurance_API>("insurance-api")
    .WaitFor(rabbitMq)
    //.WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", jaegerOtlpEndpoint)
    ;

builder.AddProject<Projects.Payment_API>("payment-api")
    .WaitFor(rabbitMq)
    //.WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", jaegerOtlpEndpoint)
    ;

builder.AddProject<Projects.Notification_API>("notification-api")
    .WaitFor(rabbitMq)
    //.WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", jaegerOtlpEndpoint)
    ;

builder.Build().Run();
