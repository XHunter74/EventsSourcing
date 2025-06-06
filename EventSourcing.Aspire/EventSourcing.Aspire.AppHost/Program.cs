var builder = DistributedApplication.CreateBuilder(args);

var username = builder.AddParameter("username", secret: true);
var password = builder.AddParameter("password", secret: true);

var postgres = builder.AddPostgres("postgres", username, password)
    .WithImage("postgres:13-alpine")
    .WithDataBindMount(source: @"..\..\Data\PostgreSQL\Data", isReadOnly: false)
    .WithPgAdmin();
var postgresdb = postgres.AddDatabase("event-sourcing");

var rabbitUsername = builder.AddParameter("rabbit-username", secret: true);
var rabbitPassword = builder.AddParameter("rabbit-password", secret: true);

var rabbitmq = builder.AddRabbitMQ("rabbit", rabbitUsername, rabbitPassword)
    .WithDataBindMount(source: @"..\..\Data\RabbitMQ\Data", isReadOnly: false)
    .WithManagementPlugin();


builder.AddProject<Projects.EventSourcing>("api")
    .WithReference(postgresdb, "DbConnection")
    .WithReference(rabbitmq, "Rabbit")
    .WaitFor(postgresdb)
    .WaitFor(rabbitmq);

builder.Build().Run();
